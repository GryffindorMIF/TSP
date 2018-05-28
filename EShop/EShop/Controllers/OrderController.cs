using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using EShop.Business;
using EShop.Business.Interfaces;
using EShop.Data;
using EShop.Models;
using EShop.Models.PostModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace EShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        private readonly int orderHistoryOrdersPerPage;
        private readonly int orderConfirmationOrdersPerPage;

        public OrderController
            (
            UserManager<ApplicationUser> userManager,
            IShoppingCartService shoppingCartService,
            IProductService productService,
            IOrderService orderService,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _shoppingCartService = shoppingCartService;
            _orderService = orderService;
            _productService = productService;

            if (!int.TryParse(configuration["PaginationConfig:OrderHistoryOrdersPerPage"], out orderHistoryOrdersPerPage))
            {
                throw new InvalidOperationException("Invalid PaginationConfig:OrderHistoryOrdersPerPage in appsettings.json. Not an int value.");
            }
            if (!int.TryParse(configuration["PaginationConfig:OrderConfirmationOrdersPerPage"], out orderConfirmationOrdersPerPage))
            {
                throw new InvalidOperationException("Invalid PaginationConfig:OrderConfirmationOrdersPerPage in appsettings.json. Not an int value.");
            }
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Index(int pageNumber = 0)
        {
            var model = new OrderHistoryModel();

            var user = await _userManager.GetUserAsync(HttpContext.User);

            //var orders = await _orderService.QueryAllOrdersAsync(user);
            var orders = _orderService.GetAllOrdersByPage(user, pageNumber, orderHistoryOrdersPerPage);

            List<OrderReview> reviews = new List<OrderReview>();

            foreach (Order o in orders)
            {
                OrderReview orm = await _orderService.FindOrderReviewAsync(o.Id);
                if (orm != null)
                {
                    orm.CustomerComment = orm.Rating + ": " + orm.CustomerComment;
                    reviews.Add(orm);
                }
            }

            model.Orders = orders;
            model.Reviews = reviews;

            // pagination
            int pageCount = _orderService.GetOrdersPageCount(user, orderHistoryOrdersPerPage);
            ViewBag.PageCount = pageCount;

            if (pageNumber + 1 < pageCount) ViewBag.NextPageNumber = pageNumber + 1;
            else ViewBag.NextPageNumber = null;
            if (pageNumber > 0) ViewBag.PreviousPageNumber = pageNumber - 1;
            else ViewBag.PreviousPageNumber = null;

            ViewBag.CurrentPageNumber = pageNumber;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<int> Repurchase([FromBody] Selector s)
        {
            // 0 - success
            // 1 - unknown exc
            // 2 - shopping-cart limit reached
            // 3 - success (bet produktai nesutampa su seniau pirktais)

            int returnCode = -1;

            try
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);

                ShoppingCart shoppingCart = await _shoppingCartService.FindShoppingCartByIdAsync((int)user.ShoppingCartId);

                Order order = await _orderService.FindOrderByIdAsync(s.SelectedValue);

                ShoppingCart oldShoppingCart = await _shoppingCartService.FindShoppingCartByIdAsync((int)order.ShoppingCartId);

                var currentProducts = await _shoppingCartService.QuerySavedProductsAsync(shoppingCart);
                //var oldProducts = await _shoppingCartService.QuerySavedProductsAsync(oldShoppingCart);
                var oldProducts = await _shoppingCartService.QueryShoppingCartProductsFromHistory(oldShoppingCart);

                //Can remove this if we also want to keep the already added products instead of repeating the order
                foreach (ShoppingCartProduct scp in currentProducts)
                {
                    await _shoppingCartService.RemoveShoppingCartProductAsync(scp.Product, shoppingCart, HttpContext.Session);
                }

                /*
                foreach (ShoppingCartProduct scp in oldProducts)
                {
                    await _shoppingCartService.AddProductToShoppingCartAsync(scp.Product, shoppingCart, scp.Quantity, HttpContext.Session);
                }
                */
                foreach(ShoppingCartProductHistory scph in oldProducts)
                {
                    // IESKOM PAGAL NAME ar dar egzistuoja toks produktas 
                    var product = await _productService.FindProductByName(scph.ProductName);
                    if (product == null)// neber prekes?
                    {
                        returnCode = 3;
                    }
                    else if(product.Price != scph.ProductPrice)// gal kaina pasikeite? pridet reik, bet pranest customeriui
                    {
                        returnCode = 3;
                        await _shoppingCartService.AddProductToShoppingCartAsync(product, shoppingCart, scph.ProductQuantity, HttpContext.Session);
                    }
                    else
                    {
                        var newReturnCode = await _shoppingCartService.AddProductToShoppingCartAsync(product, shoppingCart, scph.ProductQuantity, HttpContext.Session);
                        if (newReturnCode > returnCode) returnCode = newReturnCode;
                    }
                }
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

                OrderReview newReview = new OrderReview();

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
        public async Task<IActionResult> AdminView(int pageNumber = 0)
        {
            var model = new OrderHistoryModel();

            //var orders = await _orderService.QueryAllAdminOrdersAsync();
            var orders = _orderService.GetAllAdminOrdersByPage(pageNumber, orderConfirmationOrdersPerPage);

            List<OrderReview> reviews = new List<OrderReview>();

            foreach (Order o in orders)
            {
                OrderReview orm = await _orderService.FindOrderReviewAsync(o.Id);
                if (orm != null)
                {
                    orm.CustomerComment = orm.Rating + ": " + orm.CustomerComment;
                    reviews.Add(orm);
                }
            }

            model.Orders = orders;
            model.Reviews = reviews;

            if (TempData["AdminConfirmOrderConcurrency"] != null)
            {
                ModelState.AddModelError(string.Empty, TempData["AdminConfirmOrderConcurrency"].ToString());
            }

            // pagination
            int pageCount = _orderService.GetAdminOrdersPageCount(orderConfirmationOrdersPerPage);
            ViewBag.PageCount = pageCount;

            if (pageNumber + 1 < pageCount) ViewBag.NextPageNumber = pageNumber + 1;
            else ViewBag.NextPageNumber = null;
            if (pageNumber > 0) ViewBag.PreviousPageNumber = pageNumber - 1;
            else ViewBag.PreviousPageNumber = null;

            ViewBag.CurrentPageNumber = pageNumber;

            return View(model);
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> ConfirmOrder(int orderId, string rowVersion)
        {
            byte[] actualRowVersion = rowVersion == null ? null : Convert.FromBase64String(rowVersion);
            int confirmationResult = await _orderService.ChangeOrderConfirmationAsync(orderId, true, actualRowVersion);

            /*
            var order = await _orderService.FindOrderByIdAsync(orderId);
            var sc = await _shoppingCartService.FindShoppingCartByIdAsync((int)order.ShoppingCartId);
            await _shoppingCartService.AddShoppingCartToHistory(sc);
            */

            if (confirmationResult == -1)
            {
                TempData["AdminConfirmOrderConcurrency"] = "This order's status was already modifed after you've loaded the page";
            }

            return RedirectToAction("AdminView", "Order");
        }

        //Does nothing except rejects the order. Would do a return funds, but the mock payment does not support that function, so maybe unnecessary?
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> RejectOrder(int orderId, string rowVersion)
        {

            byte[] actualRowVersion = rowVersion == null ? null : Convert.FromBase64String(rowVersion);
            int confirmationResult = await _orderService.ChangeOrderConfirmationAsync(orderId, false, actualRowVersion);
            if (confirmationResult == -1)
            {
                TempData["AdminConfirmOrderConcurrency"] = "This order's status was already modifed after you've loaded the page";
            }
            
            return RedirectToAction("AdminView", "Order");
        }

        public async Task<IActionResult> PreviewShoppingCartHistory(int shoppingCartId)
        {
            var sc = await _shoppingCartService.FindShoppingCartByIdAsync(shoppingCartId);
            //var productsInCart = await _shoppingCartService.QueryAllShoppingCartProductsAsync(sc, HttpContext.Session);
            var productsInCart = await _shoppingCartService.QueryShoppingCartProductsFromHistory(sc);

            return View("ShoppingCartHistoryPreview", productsInCart);
        }
    }

    public class Selector
    {
        public int SelectedValue { get; set; }
    }
}