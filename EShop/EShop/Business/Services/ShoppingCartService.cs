﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShop.Business.Interfaces;
using EShop.Data;
using EShop.Models.EFModels.Product;
using EShop.Models.EFModels.ShoppingCart;
using EShop.Models.EFModels.User;
using EShop.Models.ViewModels;
using EShop.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EShop.Business.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IProductService _productService;

        private readonly int _maxProductsInShoppingCart;

        public ShoppingCartService(ApplicationDbContext context, IProductService productService,
            IConfiguration configuration)
        {
            _context = context;
            _productService = productService;

            if (!int.TryParse(configuration["ShoppingCartConfig:MaxProducts"], out _maxProductsInShoppingCart))
                throw new InvalidOperationException(
                    "Invalid ShoppingCartConfig:MaxProducts in appsettings.json. Not an int value.");
        }

        public async Task<IQueryable<ProductInCartViewModel>> QueryAllShoppingCartProductsAsync(
            ShoppingCart shoppingCart, ISession session)
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
                            ImageUrl = _context.ProductImage.FirstOrDefault(pi => pi.Product.Id == p.Id && pi.IsPrimary).ImageUrl
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
                        ImageUrl = _context.ProductImage.FirstOrDefault(pi => pi.Product.Id == p.Product.Id && pi.IsPrimary)?.ImageUrl
                    }).AsQueryable();
            }

            return (await DiscountProducts(productsInCart.ToList())).AsQueryable();
        }

        public async Task<int> CountProductsInShoppingCart(ShoppingCart shoppingCart)
        {
            return await (from scp in _context.ShoppingCartProduct
                where scp.ShoppingCart.Id == shoppingCart.Id
                select scp).CountAsync();
        }

        public async Task<int> AddProductToShoppingCartAsync(Product product, ShoppingCart cart, int quantity,
            ISession session)
        {
            if (quantity < 1)
                return 1; //Return quantity error
            if (cart != null)
            {
                var returnCode = 1;
                // 0 - success
                // 1 - error
                // 2 - limit reached
                await Task.Run(() =>
                {
                    try
                    {
                        // Check if such product has already been added
                        var shoppingCartProduct = _context.ShoppingCartProduct
                            .Where(scp => scp.Product == product && scp.ShoppingCart == cart)
                            .FirstOrDefault();

                        // No such product found
                        if (shoppingCartProduct == null)
                        {
                            var productsInCart = 0;
                            var t = Task.Run(async () => { productsInCart = await CountProductsInShoppingCart(cart); });
                            t.Wait();

                            if (productsInCart >= _maxProductsInShoppingCart)
                            {
                                returnCode = 2;
                            }
                            else
                            {
                                shoppingCartProduct = new ShoppingCartProduct
                                {
                                    Product = product,
                                    ShoppingCart = cart,
                                    Quantity = quantity
                                };

                                _context.Add(shoppingCartProduct);

                                var t2 = Task.Run(
                                    async () => { await _context.SaveChangesAsync(); });
                                t2.Wait();
                                returnCode = 0; // success
                            }
                        }
                        // Product found: update it
                        else
                        {
                            shoppingCartProduct.ShoppingCart = cart;
                            shoppingCartProduct.Quantity += quantity;


                            _context.Update(shoppingCartProduct);

                            var t2 = Task.Run(
                                async () => { await _context.SaveChangesAsync(); });
                            t2.Wait();
                            returnCode = 0; // success
                        }
                    }
                    catch (Exception)
                    {
                        returnCode = 1; // exception
                        //throw new Exception(e.ToString());
                    }
                });
                return returnCode;
            }

            if (await session.CountProducts() >= _maxProductsInShoppingCart) return 2;

            await session.AddSessionProductAsync(product.Id, quantity);
            return 0;
        }

        public async Task<int> ChangeShoppingCartProductCountAsync(Product product, ShoppingCart shoppingCart,
            string operation, ISession session)
        {
            if (shoppingCart != null)
            {
                var returnCode = 1;

                await Task.Run(() =>
                {
                    try
                    {
                        var shoppingCartProduct = _context.ShoppingCartProduct
                            .FirstOrDefault(scp => scp.Product.Name == product.Name && scp.ShoppingCart == shoppingCart);

                        //Change
                        if (shoppingCartProduct != null && (operation == "reduce" && shoppingCartProduct.Quantity > 1))
                            shoppingCartProduct.Quantity--;
                        else if (operation == "increase")
                            if (shoppingCartProduct != null)
                                shoppingCartProduct.Quantity++;

                        _context.Update(shoppingCartProduct ?? throw new InvalidOperationException());
                        _context.Update(shoppingCart);

                        var t2 = Task.Run(
                            async () => { await _context.SaveChangesAsync(); });
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

            if (operation == "reduce")
                await session.AddSessionProductAsync(product.Id, -1);
            else if (operation == "increase")
                await session.AddSessionProductAsync(product.Id, 1);
            return 0;
        }

        public async Task<int> RemoveShoppingCartProductAsync(Product product, ShoppingCart shoppingCart,
            ISession session)
        {
            if (shoppingCart != null)
            {
                var returnCode = 1;

                await Task.Run(() =>
                {
                    try
                    {
                        var shoppingCartProduct = _context.ShoppingCartProduct
                            .FirstOrDefault(scp => scp.Product.Name == product.Name && scp.ShoppingCart == shoppingCart);

                        _context.Remove(shoppingCartProduct ?? throw new InvalidOperationException());
                        _context.Update(shoppingCart);

                        var t2 = Task.Run(
                            async () => { await _context.SaveChangesAsync(); });
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

            return await session.DeleteSessionProductAsync(product.Id) ? 0 : 1;
        }

        public async Task<ShoppingCart> FindShoppingCartByIdAsync(int id)
        {
            var shoppingCart = await _context.ShoppingCart.FindAsync(id);
            return shoppingCart;
        }

        public async Task<IQueryable<ShoppingCartProduct>> QuerySavedProductsAsync(ShoppingCart shoppingCart)
        {
            IQueryable<ShoppingCartProduct> products = null;
            if (shoppingCart != null)
                await Task.Run(() =>
                {
                    products = from p in _context.Product
                        join scp in _context.ShoppingCartProduct on p.Id equals scp.Product.Id
                        join sc in _context.ShoppingCart on scp.ShoppingCart.Id equals sc.Id
                        where sc.Id == shoppingCart.Id
                        select new ShoppingCartProduct
                        {
                            Id = scp.Id,
                            Product = p,
                            ShoppingCart = sc,
                            Quantity = scp.Quantity
                        };
                });
            return ((products ?? throw new InvalidOperationException()) ?? throw new InvalidOperationException()).ToList().AsQueryable();
        }

        public async Task<ShoppingCart> CreateNewShoppingCart(HttpContext httpContext)
        {
            var shoppingCart = new ShoppingCart();
            await _context.ShoppingCart.AddAsync(shoppingCart);
            await _context.SaveChangesAsync();
            await TransferSessionProducts(httpContext, shoppingCart);
            return shoppingCart;
        }

        public async Task AssignNewShoppingCart(ApplicationUser user)
        {
            var shoppingCart = new ShoppingCart();
            await _context.ShoppingCart.AddAsync(shoppingCart);
            await _context.SaveChangesAsync();
            user.ShoppingCartId = shoppingCart.Id;
            await _context.SaveChangesAsync();
        }

        public async Task<int> TransferSessionProducts(HttpContext httpContext, ShoppingCart shoppingCart)
        {
            return await httpContext.Session.TransferSessionProductsToCartAsync(shoppingCart, _context, this);
        }

        public async Task<int> AddShoppingCartToHistory(ShoppingCart sc)
        {
            var scProducts = await QuerySavedProductsAsync(sc);

            foreach (var scProduct in scProducts)
            {
                var scph = new ShoppingCartProductHistory
                {
                    ProductName = scProduct.Product.Name,
                    ProductDescription = scProduct.Product.Description,
                    ProductPrice = scProduct.Product.Price,
                    ProductQuantity = scProduct.Quantity,
                    ShoppingCart = sc
                };

                var primaryImage =
                    await _productService.GetProductImages(scProduct.Product
                        .Id); // will always return single value (why LIST?)
                if (primaryImage.Any()) scph.ProductPrimaryImageUrl = primaryImage[0].ImageUrl;

                _context.Add(scph);
                //_context.Remove(scProduct);
            }

            await _context.SaveChangesAsync();
            return 0;
        }

        public async Task<IQueryable<ShoppingCartProductHistory>> QueryShoppingCartProductsFromHistory(ShoppingCart sc)
        {
            IQueryable<ShoppingCartProductHistory> scphs = null;

            await Task.Run(() =>
            {
                scphs = (from scph in _context.ShoppingCartProductHistory
                    where scph.ShoppingCartId == sc.Id
                    select scph).AsQueryable();
            });

            return scphs;
        }

        public async Task<IQueryable<ProductInCartViewModel>> QueryProductsInCartFromHistory(ShoppingCart sc)
        {
            var scphs = await QueryShoppingCartProductsFromHistory(sc);

            ICollection<ProductInCartViewModel> picvms = new List<ProductInCartViewModel>();

            foreach (var scph in scphs)
            {
                var product = await _productService.FindProductByName(scph.ProductName);

                var picvm = new ProductInCartViewModel
                {
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = scph.ProductQuantity,
                    TotalPrice = product.Price * scph.ProductQuantity
                };

                var productImage = await _productService.GetProductImages(product.Id);
                if (productImage.Any()) picvm.ImageUrl = productImage[0].ImageUrl;

                picvms.Add(picvm);
            }

            return picvms.AsQueryable();
        }

        public async Task<int> CalculateTotalPriceCents(ApplicationUser user, ISession session)
        {
            decimal totalCost = 0;

            //Calculate total price in backend
            if (user.ShoppingCartId != null)
            {
                var shoppingCart = await FindShoppingCartByIdAsync((int) user.ShoppingCartId);
                IEnumerable<ProductInCartViewModel> products =
                    await QueryAllShoppingCartProductsAsync(shoppingCart, session);

                foreach (var product in products) totalCost += product.Price * product.Quantity;
            }

            var totalCostCents = (int) (totalCost * 100);

            return totalCostCents;
        }

        private async Task<ICollection<ProductInCartViewModel>> DiscountProducts(
            ICollection<ProductInCartViewModel> productsInCart)
        {
            foreach (var productInCart in productsInCart)
            {
                var discountPrice =
                    await _productService.GetDiscountPrice(await _context.Product.FindAsync(productInCart.Id));
                if (discountPrice != null)
                {
                    productInCart.Price = (decimal) discountPrice;
                    productInCart.TotalPrice = productInCart.Quantity * (decimal) discountPrice;
                }
            }

            return productsInCart;
        }
    }
}