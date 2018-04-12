using EShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business
{
    public interface IShoppingCartService
    {
        Task<IQueryable<ProductInCartViewModel>> QueryAllShoppingCartProductsAsync(ApplicationUser user);
        Task<int> AddProductToShoppingCartAsync(Product product, ApplicationUser user, int quantity);
        Task<int> ChangeShoppingCartProductCountAsync(ShoppingCartProduct product, ApplicationUser user, string operation);
        Task<int> RemoveShoppingCartProductAsync(ShoppingCartProduct product, ApplicationUser user);
        // 0 - success
        // 1 - error
    }
}
