using EShop.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business
{
    public interface IShoppingCartService
    {
        Task<IQueryable<ProductInCartViewModel>> QueryAllShoppingCartProductsAsync(ShoppingCart shoppingCart, ISession session);
        Task<int> AddProductToShoppingCartAsync(Product product, ShoppingCart shoppingCart, int quantity, ISession session);
        Task<int> ChangeShoppingCartProductCountAsync(Product product, ShoppingCart shoppingCart, string operation, ISession session);
        Task<int> RemoveShoppingCartProductAsync(Product product, ShoppingCart shoppingCart, ISession session);
        Task<IQueryable<ShoppingCartProduct>> QuerySavedProductsAsync(ShoppingCart shoppingCart);
        Task<ShoppingCart> FindShoppingCartByIdAsync(int Id);
        // 0 - success
        // 1 - error
        Task<int> CountProductsInShoppingCart(ShoppingCart shoppingCart);
    }
}
