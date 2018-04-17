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
        Task<List<Category>> GetTopLevelCategoriesAsync();
        Task<List<Category>> GetChildCategoriesAsync(Category parentCategory);
        Task<ICollection<CategoryViewModel>> BuildRecursiveMenuAsync();
        Task<ICollection<CategoryViewModel>> BuildRecursiveSubMenuAsync(Category parentCategory);
        Task<ICollection<Category>> GetParentCategoriesAsync(Category childCategory);
        Task<string> RemoveLastUriSegmentAsync(string uri);
    }
}
