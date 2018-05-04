using EShop.Business;
using EShop.Business.Interfaces;
using EShop.Data;
using EShop.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Controllers
{
    public class CheckoutController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IAddressManager _addressManager;

        public CheckoutController
            (
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager, 
            IShoppingCartService shoppingCartService,
            IAddressManager addressManager
            )
        {
            _context = context;
            _userManager = userManager;
            _shoppingCartService = shoppingCartService;
            _addressManager = addressManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> Checkout()
        {
            var model = new OrderViewModel { StatusMessage = StatusMessage };
            
            ShoppingCart shoppingCart = await GetCartAsync();

            model.Products = await _shoppingCartService.QueryAllShoppingCartProductsAsync(shoppingCart, HttpContext.Session);

            return View(model);
        }

        [EnableCors("EShopCorsPolicy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakePurchase(OrderViewModel model, string totalCost)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //This crap definitely needs to be reworked
            string[] costSplit = totalCost.Split(Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
            int costOut = 0;
            Int32.TryParse(costSplit[0] + costSplit[1], out costOut);

            var user = await _userManager.GetUserAsync(HttpContext.User);

            string jsonPost =
                "{\"amount\":" + costOut +
                ",\"number\": \"" + model.CardNumber +
                "\",\"holder\": \"" + model.FirstName + " " + model.LastName +
                "\",\"exp_year\": " + model.Exp_Year +
                ",\"exp_month\": " + model.Exp_Month +
                ",\"cvv\": \"" + model.CVV + "\"}";

            //Touching anything 'Http' beyond this point is likely to result in some browsers flipping their shit out
            HttpClientHandler handler = new HttpClientHandler()
            {
                PreAuthenticate = true,
                UseDefaultCredentials = false,
            };

            HttpClient client = new HttpClient(handler);
            client.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{"technologines"}:{"platformos"}")));

            //Create POST content from data
            StringContent jContent = new StringContent(jsonPost, Encoding.UTF8, "application/json");
            jContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "https://mock-payment-processor.appspot.com/v1/payment/")
            {
                Content = jContent,
            };

            DeliveryAddress confirmAddress = await _context.DeliveryAddress.Where(da => da.Zipcode == model.ZipConfirmation).FirstOrDefaultAsync();
            if (confirmAddress != null) //If address exists
            {
                //Send payment request
                HttpResponseMessage response = await client.SendAsync(req);

                if ((int)response.StatusCode == 201) //Success
                {
                    ShoppingCart shoppingCart = null;

                    shoppingCart = await _context.ShoppingCart.FindAsync(user.ShoppingCartId);
                    user.ShoppingCartId = null;
                    _context.Update(user);

                    Order newOrder = new Order();

                    newOrder.ShoppingCart = shoppingCart;
                    newOrder.User = user;
                    newOrder.TotalPrice = Convert.ToDecimal(totalCost);
                    newOrder.Address = confirmAddress.Country + ", " + confirmAddress.County + " county, " +
                        confirmAddress.City + " - " + confirmAddress.Address + ", " + confirmAddress.Zipcode;
                    newOrder.CardNumber = model.CardNumber;
                    newOrder.PurchaseDate = DateTime.Now;
                    newOrder.StatusCode = 1; //1 - Purchased 2 - Confirmed etc.

                    _context.Order.Add(newOrder);
                    await _context.SaveChangesAsync();
                    //Todo redirect to some other page or show something
                }
            }

            //Todo something on checkout page if transaction fails etc.
            return View(nameof(Checkout));
        }

        private async Task<ShoppingCart> GetCartAsync()
        {
            ShoppingCart shoppingCart = null;

            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user != null)
            {
                //if (User.Identity.IsAuthenticated)
                //{
                if (user.ShoppingCartId != null)
                    shoppingCart = await _context.ShoppingCart.FindAsync(user.ShoppingCartId);
                if (shoppingCart == null)
                {
                    shoppingCart = new ShoppingCart();
                    _context.ShoppingCart.Add(shoppingCart);
                    await _context.SaveChangesAsync();
                    user.ShoppingCartId = shoppingCart.Id;
                    await _context.SaveChangesAsync();
                }
                //}
                //else return null;
            }
            return shoppingCart;
        }
    }
}