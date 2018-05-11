using System;
using System.Threading.Tasks;
using EShop.Business;
using EShop.Business.Interfaces;
using EShop.Data;
using EShop.Models;
using EShop.Models.PostModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOrderService _orderService;

        public OrderController
            (
            UserManager<ApplicationUser> userManager,
            IShoppingCartService shoppingCartService,
            IOrderService orderService
            )
        {
            _userManager = userManager;
            _shoppingCartService = shoppingCartService;
            _orderService = orderService;
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Index()
        {
            var model = new OrderHistoryModel();

            var user = await _userManager.GetUserAsync(HttpContext.User);

            var orders = await _orderService.QueryAllOrdersAsync(user);

            model.Orders = orders;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<int> Repurchase([FromBody] Selector s)
        {
            int returnCode = 1;

            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                ShoppingCart shoppingCart = await _shoppingCartService.FindShoppingCartByIdAsync((int)user.ShoppingCartId);

                Order order = await _orderService.FindOrderByIdAsync(s.SelectedValue);

                ShoppingCart oldShoppingCart = await _shoppingCartService.FindShoppingCartByIdAsync((int)order.ShoppingCartId);

                var currentProducts = await _shoppingCartService.QuerySavedProductsAsync(shoppingCart);
                var oldProducts = await _shoppingCartService.QuerySavedProductsAsync(oldShoppingCart);

                //Can remove this if we also want to keep the already added products instead of repeating the order
                foreach (ShoppingCartProduct scp in currentProducts)
                {
                    await _shoppingCartService.RemoveShoppingCartProductAsync(scp.Product, shoppingCart, HttpContext.Session);
                }

                foreach (ShoppingCartProduct scp in oldProducts)
                {
                    await _shoppingCartService.AddProductToShoppingCartAsync(scp.Product, shoppingCart, scp.Quantity, HttpContext.Session);
                }

                returnCode = 0;
            }
            catch (Exception)
            {
                returnCode = 1;
            }
            return returnCode;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<int> LeaveReview([FromBody] ReviewPostModel rpm)
        {
            int returnCode = 1;

            try
            {
                Order order = await _orderService.FindOrderByIdAsync(rpm.OrderId);

                OrderReviewModel newReview = new OrderReviewModel();

                var user = await _userManager.GetUserAsync(HttpContext.User);

                newReview.OrderId = order.Id;
                newReview.Rating = Convert.ToInt16(rpm.Rating);
                newReview.CustomerComment = rpm.Comment;
                newReview.User = user;
                int reviewResult = await _orderService.AddOrderReviewAsync(order, newReview);

                returnCode = 0;
            }
            catch (Exception)
            {
                returnCode = 1;
            }
            return returnCode;
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> AdminView()
        {
            var orders = await _orderService.QueryAllAdminOrdersAsync();

            return View(orders);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            int confirmationResult = await _orderService.ChangeOrderConfirmationAsync(orderId, true);

            return RedirectToAction("AdminView", "Order");
        }

        //Does nothing except rejects the order. Would do a return funds, but the mock payment does not support that function, so maybe unnecessary?
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> RejectOrder(int orderId)
        {
            int confirmationResult = await _orderService.ChangeOrderConfirmationAsync(orderId, false);

            return RedirectToAction("AdminView", "Order");
        }
    }

    public class Selector
    {
        public int SelectedValue { get; set; }
    }
}