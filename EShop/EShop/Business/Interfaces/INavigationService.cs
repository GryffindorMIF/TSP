using System.Collections.Generic;
using System.Threading.Tasks;
using EShop.Models.EFModels.Category;
using EShop.Models.EFModels.Product;
using EShop.Models.ViewModels;

namespace EShop.Business.Interfaces
{
    public interface INavigationService
    {
        Task<ICollection<Product>> GetProductsInCategoryAsync(Category category);

        Task<ICollection<Product>> GetProductsInCategoryByPageAsync(Category category, int pageNumber,
            int productsPerPage);

        Task<ICollection<Product>> GetProductsInCategoryByPageAsync(Category category, int pageNumber,
            int productsPerPage, string attributeName);

        Task<int> GetProductsInCategoryPageCount(Category category, int productsPerPage);
        Task<int> GetProductsInCategoryPageCount(Category category, int productsPerPage, string attributeName);

        Task<List<Category>> GetTopLevelCategoriesAsync();
        Task<List<Category>> GetChildCategoriesAsync(Category parentCategory);
        Task<ICollection<Category>> GetParentCategoriesAsync(Category childCategory);

        Task<Category> GetCategoryById(int? id);
        Task<Category> GetCategoryByName(string name);
        Task<IList<Category>> GetAllCategories();

        Task<ProductCategory> GetProductCategoryById(int id);
        Task<IList<ProductCategory>> GetProductCategories(int productId);
        Task<IList<ProductCategory>> GetAllProductCategories();
        Task UpdateProductCategory(ProductCategory productCategory);
        Task DeleteProductCategory(int id);

        Task AddCategory(int? parentId, string name, string description);
        Task DeleteCategory(int categoryId, bool cascade, bool refOnly, int? parentCategoryId);
        Task RenameCategory(int id, string rowVersion, string newName, string newDescription);
        Task RenameCategory(int id, string newName, string newDescription);
        Task AddTopLevelCategory(string name, string description);

        Task<ICollection<CategoryViewModel>> BuildRecursiveMenuAsync();
        Task<ICollection<CategoryViewModel>> BuildRecursiveSubMenuAsync(Category parentCategory);

        Task<string> RemoveLastUriSegmentAsync(string uri);
        Task<int> AddSubCategory(int? parentCatId, int childCatId);
    }
}