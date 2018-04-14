using EShop.Data;
using EShop.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IQueryable<ProductInCartViewModel>> QueryAllShoppingCartProductsAsync(ShoppingCart shoppingCart)
        {
            IQueryable<ProductInCartViewModel> productsInCart = null;
            if (shoppingCart != null)
                await Task.Run(() =>
                {
                    productsInCart = from p in _context.Product
                                         join scp in _context.ShoppingCartProduct on p.Id equals scp.Product.Id
                                         join sc in _context.ShoppingCart on scp.ShoppingCart.Id equals sc.Id
                                         where sc.Id == shoppingCart.Id
                                     select new ProductInCartViewModel
                                         {
                                             Name = p.Name,
                                             Price = p.Price,
                                             Quantity = scp.Quantity,
                                             TotalPrice = scp.Quantity * p.Price
                                         };
                });
            return productsInCart;
        }

        public async Task<int> AddProductToShoppingCartAsync(Product product, ShoppingCart cart, int quantity)
        {
            int returnCode = 1;
            // 0 - success
            // 1 - error

            await Task.Run(() =>
            {
                try
                {
                    // Check if such product has already been added
                    ShoppingCartProduct shoppingCartProduct = _context.ShoppingCartProduct
                        .Where(scp => scp.Product == product && scp.ShoppingCart == cart)
                        .FirstOrDefault();

                    // No such product found
                    if (shoppingCartProduct == null)
                    {
                        shoppingCartProduct = new ShoppingCartProduct
                        {
                            Product = product,
                            ShoppingCart = cart,
                            Quantity = quantity
                        };
                    }
                    // Product found: update it
                    else
                    {
                        shoppingCartProduct.ShoppingCart = cart;
                        shoppingCartProduct.Quantity += quantity;
                    }

                    _context.Update(shoppingCartProduct);

                    var t2 = Task.Run(
                        async () =>
                        {
                            await _context.SaveChangesAsync();
                        });
                    t2.Wait();
                    returnCode = 0; // success
                }
                catch (Exception)
                {
                    returnCode = 1; // exception
                    //throw new Exception(e.ToString());
                }
            });
            return returnCode;
        }

        public async Task<int> ChangeShoppingCartProductCountAsync(Product product, ShoppingCart shoppingCart, string operation)
        {
            int returnCode = 1;

            await Task.Run(() =>
            {
                try
                {
                    ShoppingCartProduct shoppingCartProduct = _context.ShoppingCartProduct
                        .Where(scp => scp.Product.Name == product.Name && scp.ShoppingCart == shoppingCart)
                        .FirstOrDefault();

                    //Change
                    if (operation == "reduce" && shoppingCartProduct.Quantity > 1)
                        shoppingCartProduct.Quantity--;
                    else if (operation == "increase")
                        shoppingCartProduct.Quantity++;

                    _context.Update(shoppingCartProduct);
                    _context.Update(shoppingCart);

                    var t2 = Task.Run(
                        async () =>
                        {
                            await _context.SaveChangesAsync();
                        });
                    t2.Wait();
                    returnCode = 0;
                }
                catch (Exception)
                {
                    returnCode = 1;
                }
            });
            return returnCode;
        }

        public async Task<int> RemoveShoppingCartProductAsync(Product product, ShoppingCart shoppingCart)
        {
            int returnCode = 1;

            await Task.Run(() =>
            {
                try
                {
                    ShoppingCartProduct shoppingCartProduct = _context.ShoppingCartProduct
                        .Where(scp => scp.Product.Name == product.Name && scp.ShoppingCart == shoppingCart)
                        .FirstOrDefault();

                    _context.Remove(shoppingCartProduct);
                    _context.Update(shoppingCart);

                    var t2 = Task.Run(
                        async () =>
                        {
                            await _context.SaveChangesAsync();
                        });
                    t2.Wait();
                    returnCode = 0;
                }
                catch (Exception)
                {
                    returnCode = 1;
                }
            });
            return returnCode;
        }
    }
}
