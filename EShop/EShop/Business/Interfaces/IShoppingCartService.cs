using System.Linq;
using System.Threading.Tasks;
using EShop.Models.EFModels.Product;
using EShop.Models.EFModels.ShoppingCart;
using EShop.Models.EFModels.User;
using EShop.Models.ViewModels;
using Microsoft.AspNetCore.Http;

namespace EShop.Business.Interfaces
{
    public interface IShoppingCartService
    {
        Task<IQueryable<ProductInCartViewModel>> QueryAllShoppingCartProductsAsync(ShoppingCart shoppingCart,
            ISession session);

        Task<int> AddProductToShoppingCartAsync(Product product, ShoppingCart shoppingCart, int quantity,
            ISession session);

        Task<int> ChangeShoppingCartProductCountAsync(Product product, ShoppingCart shoppingCart, string operation,
            ISession session);

        Task<int> RemoveShoppingCartProductAsync(Product product, ShoppingCart shoppingCart, ISession session);
        Task<IQueryable<ShoppingCartProduct>> QuerySavedProductsAsync(ShoppingCart shoppingCart);
        Task<ShoppingCart> FindShoppingCartByIdAsync(int id);

        Task<ShoppingCart> CreateNewShoppingCart(HttpContext httpContext);

        // 0 - success
        // 1 - error
        Task<int> CountProductsInShoppingCart(ShoppingCart shoppingCart);
        Task AssignNewShoppingCart(ApplicationUser user);
        Task<int> TransferSessionProducts(HttpContext httpContext, ShoppingCart shoppingCart);

        Task<int> AddShoppingCartToHistory(ShoppingCart sc);
        Task<IQueryable<ShoppingCartProductHistory>> QueryShoppingCartProductsFromHistory(ShoppingCart sc);
        Task<IQueryable<ProductInCartViewModel>> QueryProductsInCartFromHistory(ShoppingCart sc);

        Task<int> CalculateTotalPriceCents(ApplicationUser user, ISession session);
    }
}