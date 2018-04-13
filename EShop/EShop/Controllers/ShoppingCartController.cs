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

            ShoppingCart shoppingCart;

            if (user != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    shoppingCart = user.ShoppingCart;
                    if (shoppingCart == null)
                    {
                        shoppingCart = new ShoppingCart();
                        user.ShoppingCart = shoppingCart;
                        _context.ShoppingCart.Add(shoppingCart);
                        await _context.SaveChangesAsync();
                    }
                }
                else return null;
            }
            else
            {
                if (!HttpContext.Session.IsAvailable)
                    await HttpContext.Session.LoadAsync();
                byte[] carid_bytes;
                if (HttpContext.Session.TryGetValue("cartid", out carid_bytes))
                {
                    int cartid = BitConverter.ToInt32(carid_bytes, 0);
                    Console.WriteLine("cartid: " + cartid);
                    shoppingCart = await _context.ShoppingCart.FindAsync(cartid);
                }
                else
                {
                    shoppingCart = new ShoppingCart();
                    _context.ShoppingCart.Add(shoppingCart);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("new cartid: " + shoppingCart.Id);
                    HttpContext.Session.Set("cartid", BitConverter.GetBytes(shoppingCart.Id));
                }
            }
            Console.WriteLine("gotten cart id: " + shoppingCart.Id);
            return shoppingCart;
        }

        public async Task<IActionResult> AddProductToShoppingCart(int productId, int quantity)
        {
            // Product to add
            Product product = await _context.Product.FindAsync(productId);

            ShoppingCart shoppingCart = await GetCartAsync();

            Console.WriteLine("Adding product");
            int resultCode = await _shoppingCartService.AddProductToShoppingCartAsync(product, shoppingCart, quantity);
            // TODO: Implement pop-up message based on resultCode
            Console.WriteLine("Redirectint to index"); //-2147482647
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