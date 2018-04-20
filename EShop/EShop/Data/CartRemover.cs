using EShop.Data;
using EShop.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EShop.Data
{
    public class CartRemover
    {
        private static Timer _timer = null;

        public static void StartService(IWebHost host)
        {
            if (_timer == null)
                _timer = new Timer(DeleteUnusedCartsAsync, host, 0, Timeout.Infinite);
        }

        public static void StopService()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        private static void DeleteUnusedCartsAsync(object host)
        {
            IWebHost _host = (IWebHost)host;
            using (var scope = _host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var _context = services.GetRequiredService<ApplicationDbContext>();
                IQueryable<ShoppingCart> unusedCarts = from sc in _context.ShoppingCart
                                                       join usr in _context.Users on sc.Id equals usr.ShoppingCartId into usrsc
                                                       from subusr in usrsc.DefaultIfEmpty()
                                                       where subusr == default(ApplicationUser)
                                                       select sc;

                _context.ShoppingCart.RemoveRange(unusedCarts.ToArray());
                _context.SaveChanges();
            }
        }
    }
}
