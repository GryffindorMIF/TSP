using EShop.Data;
using EShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EShop.Util;

namespace EShop.Business
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<ProductInCartViewModel>> QueryAllShoppingCartProductsAsync(ShoppingCart shoppingCart, ISession session)
        {
            IQueryable<ProductInCartViewModel> productsInCart = null;
            if (shoppingCart != null)
            {
                await Task.Run(() =>
                {
                    productsInCart = from p in _context.Product
                                     join scp in _context.ShoppingCartProduct on p.Id equals scp.Product.Id
                                     join sc in _context.ShoppingCart on scp.ShoppingCart.Id equals sc.Id
                                     where sc.Id == shoppingCart.Id
                                     select new ProductInCartViewModel
                                     {
                                         Id = p.Id,
                                         Name = p.Name,
                                         Price = p.Price,
                                         Quantity = scp.Quantity,
                                         TotalPrice = scp.Quantity * p.Price,
                                         ImageUrl = _context.ProductImage.FirstOrDefault(pi => pi.Product.Id == p.Id).ImageUrl
                                     };
                });
            }
            else
            {
                var products = await session.GetProductsAsync(_context);
                productsInCart = (from p in products
                                  select new ProductInCartViewModel
                                  {
                                      Id = p.Product.Id,
                                      Name = p.Product.Name,
                                      Price = p.Product.Price,
                                      Quantity = p.Count,
                                      TotalPrice = p.Count * p.Product.Price,
                                      ImageUrl = _context.ProductImage.FirstOrDefault(pi => pi.Product.Id == p.Product.Id).ImageUrl
                                  }).AsQueryable();
            }
            return productsInCart;
        }

        public async Task<int> AddProductToShoppingCartAsync(Product product, ShoppingCart cart, int quantity, ISession session)
        {
            if (cart != null)
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
            else
            {
                await session.AddSessionProductAsync(product.Id, quantity);
                return 0;
            }
        }

        public async Task<int> ChangeShoppingCartProductCountAsync(Product product, ShoppingCart shoppingCart, string operation, ISession session)
        {
            if (shoppingCart != null)
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
            else
            {
                if (operation == "reduce")
                    await session.AddSessionProductAsync(product.Id, -1);
                else if (operation == "increase")
                    await session.AddSessionProductAsync(product.Id, 1);
                return 0;
            }
        }

        public async Task<int> RemoveShoppingCartProductAsync(Product product, ShoppingCart shoppingCart, ISession session)
        {
            if (shoppingCart != null)
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
            else
            {
                return await session.DeleteSessionProductAsync(product.Id) ? 0 : 1;
            }
        }
    }
}
