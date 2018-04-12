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

namespace EShop.Controllers
{
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

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Index()
        {
            // Get current user
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var productsInCart = await _shoppingCartService.QueryAllShoppingCartProductsAsync(user);

            return View(productsInCart);
        }
        public async Task<IActionResult> AddProductToShoppingCart(int productId, int quantity)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Get current user
                var user = await _userManager.GetUserAsync(HttpContext.User);
                // Product to add
                Product product = await _context.Product.FindAsync(productId);

                int resultCode = await _shoppingCartService.AddProductToShoppingCartAsync(product, user, quantity);
                // Implement pop-up message based on resultCode

                return RedirectToAction("Index", "Home", await _context.Product.ToListAsync());
            }
            else return RedirectToAction("Register", "Account");
        }
        public async Task<IActionResult> ChangeProductCount(string productName, string operation)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            int resultCode = await _shoppingCartService.ChangeProductCountAsync(productName, user, operation);

            return RedirectToAction("Index", "ShoppingCart");
        }
    }
}