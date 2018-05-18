using System;
using System.Collections.Generic;
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
using Microsoft.AspNetCore.Http;
using EShop.Util;
using EShop.Extensions;
using Microsoft.Extensions.Configuration;

namespace EShop.Controllers
{
    [AllowAnonymous]
    [DenyAccess(Roles = "Admin, SuperAdmin")]
    public class ShoppingCartController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductService _productService;

        public ShoppingCartController(UserManager<ApplicationUser> userManager, IShoppingCartService shoppingCartService, IProductService productService)
        {
            _userManager = userManager;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
        }
       
        public async Task<IActionResult> Index()
        {
            ShoppingCart shoppingCart = await GetCartAsync();

            var productsInCart = await _shoppingCartService.QueryAllShoppingCartProductsAsync(shoppingCart, HttpContext.Session);

            return View(productsInCart);
        }

        private async Task<ShoppingCart> GetCartAsync()
        {
            ShoppingCart shoppingCart = null;

            // Get current user
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user != null)
            {
                if (user.ShoppingCartId != null)
                    shoppingCart = await _shoppingCartService.FindShoppingCartByIdAsync((int)user.ShoppingCartId);
                if (shoppingCart == null)
                {
                    await _shoppingCartService.AssignNewShoppingCart(user);
                }
            }
            return shoppingCart;
        }
        // AJAX action
        [HttpPost]
        public async Task<IActionResult> AddProductToShoppingCart([FromBody]ProductToCartPostModel productToCartPostModel)// Encapsulated post model for AJAX request
        {
            Product product = await _productService.FindProductByIdAsync(productToCartPostModel.ProductId);

            ShoppingCart shoppingCart = await GetCartAsync();

            int resultCode = await _shoppingCartService.AddProductToShoppingCartAsync(product, shoppingCart, productToCartPostModel.Quantity, HttpContext.Session);
            return Json(resultCode);// AJAX handles pop-up modal based on this return
        }

        public async Task<IActionResult> ChangeShoppingCartProductCount(string productName, string operation)
        {
            ShoppingCart shoppingCart = await GetCartAsync();

            Product product = await _productService.FindProductByName(productName);
            int resultCode = await _shoppingCartService.ChangeShoppingCartProductCountAsync(product, shoppingCart, operation, HttpContext.Session);

            return RedirectToAction("Index", "ShoppingCart");
        }

        public async Task<IActionResult> RemoveShoppingCartProduct(string productName)
        {
            ShoppingCart shoppingCart = await GetCartAsync();

            Product product = await _productService.FindProductByName(productName);
            int resultCode = await _shoppingCartService.RemoveShoppingCartProductAsync(product, shoppingCart, HttpContext.Session);

            return RedirectToAction("Index", "ShoppingCart");
        }
        
        [HttpGet]
        public async Task<int> AddSessionProductsToCartAsync()
        {
            var shoppingCart = await GetCartAsync();
            return await _shoppingCartService.TransferSessionProducts(HttpContext, shoppingCart);
        }

        [HttpGet]
        public void DiscardSessionProducts()
        {
            HttpContext.Session.ClearProducts();
        }
    }
}