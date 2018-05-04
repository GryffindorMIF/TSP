using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShop.Data;
using EShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController
            (
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager
            )
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var orders = await QueryAllOrders(user);

            return View(orders);
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
                                      ShoppingCart = o.ShoppingCart,
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
    }
}