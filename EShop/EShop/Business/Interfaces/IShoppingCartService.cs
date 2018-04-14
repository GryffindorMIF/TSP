using EShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business
{
    public interface IShoppingCartService
    {
        Task<IQueryable<ProductInCartViewModel>> QueryAllShoppingCartProductsAsync(ShoppingCart shoppingCart);
        Task<int> AddProductToShoppingCartAsync(Product product, ShoppingCart shoppingCart, int quantity);
        Task<int> ChangeShoppingCartProductCountAsync(Product product, ShoppingCart shoppingCart, string operation);
        Task<int> RemoveShoppingCartProductAsync(Product product, ShoppingCart shoppingCart);
        // 0 - success
        // 1 - error
    }
}
