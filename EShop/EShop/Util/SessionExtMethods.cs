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
        public struct SessionProducts
        {
            public int ID { get; set; }
            public int Count { get; set; }
        }

        public static async Task<List<SessionProducts>> GetSessionProductsAsync(this ISession session)
        {
            if (session.IsAvailable)
                await session.LoadAsync();
            string products = session.GetString("products");
            if (string.IsNullOrWhiteSpace(products))
                return new List<SessionProducts>();
            return JsonConvert.DeserializeObject<List<SessionProducts>>(products);
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
            session.Remove("products");
        }

        private static void SaveSessionProducts(this ISession session, List<SessionProducts> products)
        {
            session.SetString("products", JsonConvert.SerializeObject(products));
        }
    }
}
