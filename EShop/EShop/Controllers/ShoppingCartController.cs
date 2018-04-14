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

        public async Task<IActionResult> Index()
        {
            ShoppingCart shoppingCart = await GetCartAsync();

            var productsInCart = await _shoppingCartService.QueryAllShoppingCartProductsAsync(shoppingCart);

            return View(productsInCart);

        }

        private async Task<ShoppingCart> GetCartAsync()
        {
            // Get current user
            var user = await _userManager.GetUserAsync(HttpContext.User);

            ShoppingCart shoppingCart = null;

            if (user != null)
            {
                if (User.Identity.IsAuthenticated)
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
                else return null;
            }
            else
            {
                if (!HttpContext.Session.IsAvailable)
                    await HttpContext.Session.LoadAsync();
                int? cartid = HttpContext.Session.GetInt32("cartid");
                if (cartid.HasValue)
                    shoppingCart = await _context.ShoppingCart.FindAsync(cartid);
                else
                {
                    shoppingCart = new ShoppingCart();
                    _context.ShoppingCart.Add(shoppingCart);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetInt32("cartid", shoppingCart.Id);
                }
            }
            return shoppingCart;
        }
        public async Task<IActionResult> AddProductToShoppingCart(int productId, int quantity)
        {
            // Product to add
            Product product = await _context.Product.FindAsync(productId);

            ShoppingCart shoppingCart = await GetCartAsync();

            int resultCode = await _shoppingCartService.AddProductToShoppingCartAsync(product, shoppingCart, quantity);
            // TODO: Implement pop-up message based on resultCode

            return RedirectToAction("Index", "Home", await _context.Product.ToListAsync());
        }

        public async Task<IActionResult> ChangeShoppingCartProductCount(string productName, string operation)
        {
            ShoppingCart shoppingCart = await GetCartAsync();

            Product product = await _context.Product.Where(p => p.Name == productName).FirstOrDefaultAsync();
            int resultCode = await _shoppingCartService.ChangeShoppingCartProductCountAsync(product, shoppingCart, operation);

            return RedirectToAction("Index", "ShoppingCart");
        }

        public async Task<IActionResult> RemoveShoppingCartProduct(string productName)
        {
            ShoppingCart shoppingCart = await GetCartAsync();

            Product product = await _context.Product.Where(p => p.Name == productName).FirstOrDefaultAsync();
            int resultCode = await _shoppingCartService.RemoveShoppingCartProductAsync(product, shoppingCart);

            return RedirectToAction("Index", "ShoppingCart");
        }
    }
}