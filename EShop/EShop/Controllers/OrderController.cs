using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EShop.Business;
using EShop.Data;
using EShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IShoppingCartService _shoppingCartService;

        public OrderController
            (
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IShoppingCartService shoppingCartService
            )
        {
            _context = context;
            _userManager = userManager;
            _shoppingCartService = shoppingCartService;
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Index()
        {
            var model = new OrderHistoryModel();

            var user = await _userManager.GetUserAsync(HttpContext.User);

            var orders = await QueryAllOrders(user);

            model.Orders = orders;

            return View(model);
        }

        [Authorize(Roles = "Customer")]
        public async Task<IQueryable<Order>> QueryAllOrders(ApplicationUser user)
        {
            IQueryable<Order> savedOrders = null;
            if (user != null)
                await Task.Run(() =>
                {
                    savedOrders = from o in _context.Order
                                  where o.User.Id == user.Id
                                  select new Order
                                  {
                                      Id = o.Id,
                                      ShoppingCartId = o.ShoppingCartId,
                                      TotalPrice = o.TotalPrice,
                                      Address = o.Address,
                                      User = user,
                                      CardNumber = o.CardNumber,
                                      PurchaseDate = o.PurchaseDate,
                                      ConfirmationDate = o.ConfirmationDate,
                                      StatusCode = o.StatusCode
                                  };
                });
            return savedOrders;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<int> Repurchase([FromBody] Selector s)
        {
            int returnCode;

            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                ShoppingCart shoppingCart = await _context.ShoppingCart.FindAsync(user.ShoppingCartId);

                Order order = await _context.Order.FindAsync(s.SelectedValue);

                ShoppingCart oldShoppingCart = await _context.ShoppingCart.FindAsync(order.ShoppingCartId);

                var currentProducts = await _shoppingCartService.QueryAllShoppingCartProductsAsync(shoppingCart, HttpContext.Session);
                var oldProducts = await _shoppingCartService.QueryAllShoppingCartProductsAsync(oldShoppingCart, HttpContext.Session);

                //Can remove this if we also want to keep the already added products instead of repeating the order
                foreach (ProductInCartViewModel pc in currentProducts)
                {
                    Product product = await _context.Product.Where(p => p.Name == pc.Name).FirstOrDefaultAsync();

                    await _shoppingCartService.RemoveShoppingCartProductAsync(product, shoppingCart, HttpContext.Session);
                }

                foreach (ProductInCartViewModel pc in oldProducts)
                {
                    Product product = await _context.Product.Where(p => p.Name == pc.Name).FirstOrDefaultAsync();

                    await _shoppingCartService.AddProductToShoppingCartAsync(product, shoppingCart, pc.Quantity, HttpContext.Session);
                }
            }
            catch (Exception)
            {
                returnCode = 1;
            }


            return 0;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminView()
        {
            var orders = await QueryUnconfirmedOrders();

            return View(orders);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IQueryable<Order>> QueryUnconfirmedOrders()
        {
            IQueryable<Order> savedOrders = null;
                await Task.Run(() =>
                {
                    savedOrders = from o in _context.Order
                                  where o.StatusCode == 1
                                  select new Order
                                  {
                                      Id = o.Id,
                                      ShoppingCartId = o.ShoppingCartId,
                                      TotalPrice = o.TotalPrice,
                                      Address = o.Address,
                                      User = o.User,
                                      CardNumber = o.CardNumber,
                                      PurchaseDate = o.PurchaseDate,
                                      ConfirmationDate = o.ConfirmationDate,
                                      StatusCode = o.StatusCode
                                  };
                });
            return savedOrders;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            Order order = null;

            order = await _context.Order.FindAsync(orderId);

            order.StatusCode = 2;
            order.ConfirmationDate = DateTime.Now;

            _context.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("AdminView", "Order");
        }

        //Does nothing except rejects the order. Would do a return funds, but the mock payment does not support that function, so maybe unnecessary?
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectOrder(int orderId)
        {
            Order order = null;

            order = await _context.Order.FindAsync(orderId);

            order.StatusCode = 3;
            order.ConfirmationDate = DateTime.Now;

            _context.Update(order);
            await _context.SaveChangesAsync();

            return RedirectToAction("AdminView", "Order");
        }
    }

    public class Selector
    {
        public int SelectedValue { get; set; }
    }
}