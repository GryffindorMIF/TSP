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

namespace EShop.Controllers
{
    [AllowAnonymous]
    [DenyAccess(Roles = "Admin, SuperAdmin")]
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IShoppingCartService shoppingCartService)
        {
            _context = context;
            _userManager = userManager;
            _shoppingCartService = shoppingCartService;
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
                    shoppingCart = await _context.ShoppingCart.FindAsync(user.ShoppingCartId);
                if (shoppingCart == null)
                {
                    shoppingCart = new ShoppingCart();
                    _context.ShoppingCart.Add(shoppingCart);
                    await _context.SaveChangesAsync();
                    user.ShoppingCartId = shoppingCart.Id;
                    await _context.SaveChangesAsync();
                }
            }
            return shoppingCart;
        }
        // AJAX action
        [HttpPost]
        public async Task<IActionResult> AddProductToShoppingCart([FromBody]ProductToCartPostModel productToCartPostModel)// Encapsulated post model for AJAX request
        {
            Product product = await _context.Product.FindAsync(productToCartPostModel.ProductId);

            ShoppingCart shoppingCart = await GetCartAsync();

            int resultCode = await _shoppingCartService.AddProductToShoppingCartAsync(product, shoppingCart, productToCartPostModel.Quantity, HttpContext.Session);
            return Json(resultCode);// AJAX handles pop-up modal based on this return
        }

        public async Task<IActionResult> ChangeShoppingCartProductCount(string productName, string operation)
        {
            ShoppingCart shoppingCart = await GetCartAsync();

            Product product = await _context.Product.Where(p => p.Name == productName).FirstOrDefaultAsync();
            int resultCode = await _shoppingCartService.ChangeShoppingCartProductCountAsync(product, shoppingCart, operation, HttpContext.Session);

            return RedirectToAction("Index", "ShoppingCart");
        }

        public async Task<IActionResult> RemoveShoppingCartProduct(string productName)
        {
            ShoppingCart shoppingCart = await GetCartAsync();

            Product product = await _context.Product.Where(p => p.Name == productName).FirstOrDefaultAsync();
            int resultCode = await _shoppingCartService.RemoveShoppingCartProductAsync(product, shoppingCart, HttpContext.Session);

            return RedirectToAction("Index", "ShoppingCart");
        }
        
        [HttpGet]
        public async Task AddSessionProductsToCartAsync()
        {
            var shoppingCart = await GetCartAsync();
            await HttpContext.Session.TransferSessionProductsToCartAsync(shoppingCart, _context, _shoppingCartService);
        }

        [HttpGet]
        public void DiscardSessionProducts()
        {
            HttpContext.Session.ClearProducts();
        }
    }
}