using EShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business
{
    public interface INavigationService
    {
        Task<ICollection<Product>> GetProductsInCategoryAsync(Category category);
        Task<ICollection<Product>> GetProductsInCategoryByPageAsync(Category category, int pageNumber, int productsPerPage);
        Task<ICollection<Product>> GetProductsInCategoryByPageAsync(Category category, int pageNumber, int productsPerPage, string attributeName);
        Task<int> GetProductsInCategoryPageCount(Category category, int productsPerPage);
        Task<int> GetProductsInCategoryPageCount(Category category, int productsPerPage, string attributeName);
        Task<List<Category>> GetTopLevelCategoriesAsync();
        Task<List<Category>> GetChildCategoriesAsync(Category parentCategory);
        Task<ICollection<CategoryViewModel>> BuildRecursiveMenuAsync();
        Task<ICollection<CategoryViewModel>> BuildRecursiveSubMenuAsync(Category parentCategory);
        Task<ICollection<Category>> GetParentCategoriesAsync(Category childCategory);
        Task<string> RemoveLastUriSegmentAsync(string uri);
    }
}
