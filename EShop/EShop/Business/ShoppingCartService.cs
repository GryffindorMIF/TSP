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

        public async Task<IQueryable<ProductInCartViewModel>> QueryAllShoppingCartProductsAsync(ApplicationUser user)
        {
            IQueryable<ProductInCartViewModel> productsInCart = null;
            await Task.Run(() =>
            {
                productsInCart = from p in _context.Product
                                     join scp in _context.ShoppingCartProduct on p.Id equals scp.Product.Id
                                     join sc in _context.ShoppingCart on scp.ShoppingCart.Id equals sc.Id
                                     where sc.Id == user.ShoppingCartId
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

        public async Task<int> AddProductToShoppingCartAsync(Product product, ApplicationUser user, int quantity)
        {
            int returnCode = 1;
            // 0 - success
            // 1 - error

            await Task.Run(() =>
            {
                try
                {
                    ShoppingCart shoppingCart = null;

                    // Check if user has a cart
                    if (user.ShoppingCartId == null)
                    {
                        shoppingCart = new ShoppingCart();
                        user.ShoppingCart = shoppingCart;
                        _context.Add(shoppingCart);
                    }
                    else
                    {
                        var t1 = Task.Run(
                            async () =>
                            {
                                shoppingCart = await _context.ShoppingCart.FindAsync(user.ShoppingCartId);
                            });
                        t1.Wait();
                    }

                    // Check if such product has already been added
                    ShoppingCartProduct shoppingCartProduct = _context.ShoppingCartProduct
                        .Where(sc => sc.Product == product && sc.ShoppingCart == shoppingCart)
                        .FirstOrDefault();

                    // No such product found
                    if (shoppingCartProduct == null)
                    {
                        shoppingCartProduct = new ShoppingCartProduct
                        {
                            Product = product,
                            ShoppingCart = shoppingCart,
                            Quantity = quantity
                        };
                    }
                    // Product found: update it
                    else
                    {
                        shoppingCartProduct.ShoppingCart = shoppingCart;
                        shoppingCartProduct.Quantity += quantity;
                    }

                    _context.Update(shoppingCartProduct);
                    _context.Update(user);

                    var t2 = Task.Run(
                        async () =>
                        {
                            await _context.SaveChangesAsync();
                        });
                    t2.Wait();
                    returnCode = 0; // success
                }
                catch (Exception e)
                {
                    returnCode = 1; // exception
                    //throw new Exception(e.ToString());
                }
            });
            return returnCode;
        }

        public async Task<int> ChangeProductCountAsync(string productName, ApplicationUser user, string operation)
        {
            int returnCode = 1;

            await Task.Run(() =>
            {
                try
                {
                    ShoppingCart shoppingCart = null;

                    var t1 = Task.Run(
                        async () =>
                        {
                            shoppingCart = await _context.ShoppingCart.FindAsync(user.ShoppingCartId);
                        });
                    t1.Wait();

                    //Selecting product by name, maybe fix this? ProductInCartViewModel can only be distinguished by Name
                    ShoppingCartProduct product = _context.ShoppingCartProduct
                        .Where(sc => sc.Product.Name == productName && sc.ShoppingCart == shoppingCart)
                        .FirstOrDefault();

                    //Change
                    if (operation == "reduce" && product.Quantity > 1)  
                        product.Quantity--;
                    else if (operation == "increase")
                        product.Quantity++;

                    _context.Update(product);
                    _context.Update(user);

                    var t2 = Task.Run(
                        async () =>
                        {
                            await _context.SaveChangesAsync();
                        });
                    t2.Wait();
                    returnCode = 0; // success
                }
                catch (Exception e)
                {
                    returnCode = 1; // exception
                    //throw new Exception(e.ToString());
                }
            });
            return returnCode;
        }
    }
}
