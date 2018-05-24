using EShop.Business;
using EShop.Business.Interfaces;
using EShop.Data;
using EShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CheckoutController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IAddressManager _addressManager;
        private readonly IOrderService _orderService;

        public CheckoutController
            (
            UserManager<ApplicationUser> userManager, 
            IShoppingCartService shoppingCartService,
            IAddressManager addressManager,
            IOrderService orderService
            )
        {
            _userManager = userManager;
            _shoppingCartService = shoppingCartService;
            _addressManager = addressManager;
            _orderService = orderService;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> Checkout()
        {
            var model = new OrderViewModel { StatusMessage = StatusMessage };
            var user = await _userManager.GetUserAsync(User);
            var savedAddresses = await _addressManager.QueryAllSavedDeliveryAddresses(user);
            ShoppingCart shoppingCart = await _shoppingCartService.FindShoppingCartByIdAsync((int)user.ShoppingCartId);

            model.Products = await _shoppingCartService.QueryAllShoppingCartProductsAsync(shoppingCart, HttpContext.Session);

            model.savedAddresses = new List<SelectListItem>();

            foreach (DeliveryAddress da in savedAddresses)
            {
                model.savedAddresses.Add(new SelectListItem
                {
                    Text = da.Zipcode,
                    Value = da.Zipcode
                });
            }

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

            Debug.WriteLine("ZipConf is: " + model.ZipConfirmation);
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

            DeliveryAddress confirmAddress = await _addressManager.FindAddressByZipcodeAsync(model.ZipConfirmation);
            if (confirmAddress != null) //If address exists
            {
                //Send payment request
                HttpResponseMessage response = await client.SendAsync(req);

                if ((int)response.StatusCode == 201) //Success
                {
                    ShoppingCart shoppingCart = null;

                    shoppingCart = await _shoppingCartService.FindShoppingCartByIdAsync((int)user.ShoppingCartId);
                    await _shoppingCartService.AssignNewShoppingCart(user);
                    //user.ShoppingCartId = null;
                    //await _userManager.UpdateAsync(user);

                    Order newOrder = new Order();

                    newOrder.ShoppingCartId = shoppingCart.Id;
                    newOrder.UserId = user.Id;
                    newOrder.TotalPrice = Convert.ToDecimal(totalCost);
                    newOrder.Address = confirmAddress.Country + ", " + confirmAddress.County + " county, " +
                    confirmAddress.City + " - " + confirmAddress.Address + ", " + confirmAddress.Zipcode;
                    newOrder.CardNumber = model.CardNumber;
                    newOrder.PurchaseDate = DateTime.Now;
                    newOrder.StatusCode = 1; //1 - Purchased 2 - Confirmed etc.

                    int addOrderResult = await _orderService.CreateOrderAsync(newOrder);

                    await _shoppingCartService.AddShoppingCartToHistory(shoppingCart);
                    //Todo redirect to some other page or show something
                }
            }         

            //Todo something on checkout page if transaction fails etc.
            return RedirectToAction("Index", "Order");
        }
    }
}