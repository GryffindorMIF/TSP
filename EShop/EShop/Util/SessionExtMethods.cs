using EShop.Business;
using EShop.Data;
using EShop.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Util
{
    public static class SessionExtMethods
    {
        public const string PRODUCTS_KEY = "products";

        private struct SessionProducts
        {
            public int ID { get; set; }
            public int Count { get; set; }
        }

        public struct Products
        {
            public Product Product { get; set; }
            public int Count { get; set; }
        }

        private static async Task<List<SessionProducts>> GetSessionProductsAsync(this ISession session)
        {
            if (session.IsAvailable)
                await session.LoadAsync();
            string products = session.GetString(PRODUCTS_KEY);
            if (string.IsNullOrWhiteSpace(products))
                return new List<SessionProducts>();
            return JsonConvert.DeserializeObject<List<SessionProducts>>(products);
        }

        public static async Task<List<Products>> GetProductsAsync(this ISession session, ApplicationDbContext context)
        {
            List<Products> products = new List<Products>();
            List<SessionProducts> sessionProducts = await session.GetSessionProductsAsync();

            products = (from sp in sessionProducts
                        join p in context.Product on sp.ID equals p.Id
                        select new Products { Product = p, Count = sp.Count }
                        ).ToList();

            return products;
        }

        public static async Task AddSessionProductAsync(this ISession session, int productId, int count)
        {
            List<SessionProducts> products = await session.GetSessionProductsAsync();
            bool done = false;
            for (int i = 0; i < products.Count; i++)
                if (products[i].ID == productId)
                {
                    if (products[i].Count + count <= 0)
                        return;
                    products[i] = new SessionProducts() { ID = productId, Count = products[i].Count + count };
                    done = true;
                    break;
                }
            if (!done)
                products.Add(new SessionProducts() { ID = productId, Count = count });
            session.SaveSessionProducts(products);
        }

        public static async Task<bool> DeleteSessionProductAsync(this ISession session, int productId)
        {
            List<SessionProducts> products = await session.GetSessionProductsAsync();
            for (int i = 0; i < products.Count; i++)
                if (products[i].ID == productId)
                {
                    products.RemoveAt(i);
                    session.SaveSessionProducts(products);
                    return true;
                }
            return false;
        }

        public static void ClearProducts(this ISession session)
        {
            session.Remove(PRODUCTS_KEY);
        }

        private static void SaveSessionProducts(this ISession session, List<SessionProducts> products)
        {
            if (products.Count == 0)
                session.Remove(PRODUCTS_KEY);
            else session.SetString(PRODUCTS_KEY, JsonConvert.SerializeObject(products));
        }

        public static async Task<int> TransferSessionProductsToCartAsync(this ISession session, ShoppingCart shoppingCart, ApplicationDbContext context, IShoppingCartService shoppingCartService)
        {
            var products = await session.GetProductsAsync(context);
            foreach (var product in products)
            {
                int result = await shoppingCartService.AddProductToShoppingCartAsync(product.Product, shoppingCart, product.Count, session);
                if (result == 2 || result == 1)
                {
                    return result;// 2 == product limit reached || 1 == unknown error
                }
            }
            session.ClearProducts();
            return 0;// success
        }

        public static async Task<int> CountProducts(this ISession session)
        {
            List<SessionProducts> products = await session.GetSessionProductsAsync();
            return products.Count();
        }
    }
}
