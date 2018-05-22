using EShop.Data;
using EShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business
{
    public class NavigationService : INavigationService
    {
        private readonly ApplicationDbContext _context;

        public NavigationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Product>> GetProductsInCategoryAsync(Category category)
        {
            ICollection<Product> productsInCategory = null;
            await Task.Run(() =>
            {
                productsInCategory = (from p in _context.Product
                                      join pc in _context.ProductCategory on p.Id equals pc.ProductId
                                      where pc.CategoryId == category.Id
                                      select p).ToList();
            });
            return productsInCategory;
        }

        public async Task<ICollection<Product>> GetProductsInCategoryByPageAsync(Category category, int pageNumber, int productsPerPage)
        {
            ICollection<Product> productsInCategory = null;

            await Task.Run(() =>
            {
                if (category != null)
                {
                    productsInCategory = (from p in _context.Product
                                          join pc in _context.ProductCategory on p.Id equals pc.ProductId
                                          where pc.CategoryId == category.Id
                                          select p).Skip(pageNumber * productsPerPage).Take(productsPerPage).ToList();
                }
                else
                {
                    productsInCategory = _context.Product.Skip(pageNumber * productsPerPage).Take(productsPerPage).ToList();
                }
            });

            return productsInCategory;
        }

        public async Task<ICollection<Product>> GetProductsInCategoryByPageAsync(Category category, int pageNumber, int productsPerPage, string attributeName)
        {
            ICollection<Product> productsInCategory = null;

            await Task.Run(() =>
            {
                productsInCategory = (from p in _context.Product
                                      from a in _context.AttributeValue
                                      join pc in _context.ProductCategory on p.Id equals pc.ProductId
                                      join pa in _context.ProductAttributeValue on p.Id equals pa.ProductId
                                      where pc.CategoryId == category.Id
                                      where pa.AttributeValueId == a.Id
                                      where a.Name == attributeName
                                      select p).Skip(pageNumber * productsPerPage).Take(productsPerPage).ToList();
            });

            return productsInCategory;
        }

        public async Task<int> GetProductsInCategoryPageCount(Category category, int productsPerPage)
        {
            int productsTotalCount;
            int pageCount = 0;

            await Task.Run(() =>
            {
                if (category != null)
                {
                    productsTotalCount = (from p in _context.Product
                                          join pc in _context.ProductCategory on p.Id equals pc.ProductId
                                          where pc.CategoryId == category.Id
                                          select p).Count();
                }
                else
                {
                    productsTotalCount = _context.Product.Count();
                }

                pageCount = productsTotalCount / productsPerPage;

                if (productsTotalCount % productsPerPage != 0)
                {
                    pageCount++;
                }

            });
            return pageCount;
        }

        public async Task<int> GetProductsInCategoryPageCount(Category category, int productsPerPage, string attributeName)
        {
            int productsTotalCount;
            int pageCount = 0;

            await Task.Run(() =>
            {
                if (category != null)
                {
                    productsTotalCount = (from p in _context.Product
                                          from a in _context.AttributeValue
                                          join pc in _context.ProductCategory on p.Id equals pc.ProductId
                                          join pa in _context.ProductAttributeValue on p.Id equals pa.ProductId
                                          where pc.CategoryId == category.Id
                                          where pa.AttributeValueId == a.Id
                                          where a.Name == attributeName
                                          select p).Count();
                }
                else
                {
                    productsTotalCount = _context.Product.Count();
                }

                pageCount = productsTotalCount / productsPerPage;

                if (productsTotalCount % productsPerPage != 0)
                {
                    pageCount++;
                }

            });
            return pageCount;
        }

        public async Task<List<Category>> GetTopLevelCategoriesAsync()
        {
            List<Category> topLevelCategories = null;
            await Task.Run(() =>
            {
                topLevelCategories = (from cc in _context.CategoryCategory
                                      from c in _context.Category
                                      where cc.ParentCategoryId == null
                                      where c.Id == cc.CategoryId
                                      select c).ToList();
            });
            return topLevelCategories;
        }

        public async Task<IList<Category>> GetAllCategories()
        {
            return await _context.Category.ToListAsync();
        }

        public async Task<IList<ProductCategory>> GetProductCategories(int productId)
        {
            return await (from pc in _context.ProductCategory
                          where pc.ProductId == productId
                          select pc).ToListAsync();
        }

        public async Task<List<Category>> GetChildCategoriesAsync(Category parentCategory)
        {
            if (parentCategory == null)
            {
                Debug.WriteLine("Nullexception occurred");
                StackTrace stackTrace = new StackTrace();

                // Get calling method name
                Debug.WriteLine(stackTrace);
            }
            List<Category> childCategories = null;
            await Task.Run(() =>
            {
                childCategories = (from cc in _context.CategoryCategory
                                   from c in _context.Category
                                   where cc.ParentCategoryId == parentCategory.Id
                                   where c.Id == cc.CategoryId
                                   select c).ToList();
            });
            return childCategories;
        }

        public async Task<ICollection<CategoryViewModel>> BuildRecursiveMenuAsync()
        {
            List<Category> topLevelCategories = await GetTopLevelCategoriesAsync();
            ICollection<CategoryViewModel> categoryViewModels = new List<CategoryViewModel>();

            foreach (var topLevelCategory in topLevelCategories)
            {
                CategoryViewModel categoryViewModel = new CategoryViewModel()
                {
                    Category = topLevelCategory,
                    SubCategories = await BuildRecursiveSubMenuAsync(topLevelCategory)
                };
                categoryViewModels.Add(categoryViewModel);
            }
            return categoryViewModels;
        }

        public async Task<ICollection<CategoryViewModel>> BuildRecursiveSubMenuAsync(Category parentCategory)
        {
            List<Category> childCategories = await GetChildCategoriesAsync(parentCategory);
            ICollection<CategoryViewModel> categoryViewModels = new List<CategoryViewModel>();

            if (childCategories.Count > 0)
            {
                foreach (var childCategory in childCategories)
                {
                    CategoryViewModel categoryViewModel = new CategoryViewModel()
                    {
                        Category = childCategory,
                        SubCategories = await BuildRecursiveSubMenuAsync(childCategory)// recursion
                    };
                    categoryViewModels.Add(categoryViewModel);
                }
            }
            return categoryViewModels;
        }

        public async Task<ICollection<Category>> GetParentCategoriesAsync(Category childCategory)
        {
            ICollection<int?> parentCategoriesIds = await (from cc in _context.CategoryCategory
                                                           where cc.CategoryId == childCategory.Id
                                                           select cc.ParentCategoryId).ToListAsync();

            ICollection<Category> parentCategories = new List<Category>();

            foreach (int? parentCategoryId in parentCategoriesIds)
            {
                Category parentCategory = null;
                if (parentCategoryId != null) parentCategory = await _context.Category.FindAsync(parentCategoryId);
                if (parentCategory != null) parentCategories.Add(parentCategory);
            }
            return parentCategories;
        }

        public async Task<string> RemoveLastUriSegmentAsync(string uri)
        {
            await Task.Run(() =>
            {
                string[] pathSegments = uri.Split("/");
                uri = "";
                for (int i = 0; i < pathSegments.Length - 1; i++)
                {
                    if (i == 0) uri += pathSegments[i];
                    else uri += ("/" + pathSegments[i]);
                }
            });
            return uri;
        }

        public async Task<Category> GetCategoryById(int? Id)
        {
            Category category = await _context.Category.FindAsync(Id);
            return category;
        }

        public async Task<Category> GetCategoryByName(string name)
        {
            Category category = await (from c in _context.Category
                                       where c.Name == name
                                       select c).FirstAsync();// category name is unique
            return category;
        }

        public async Task AddCategory(int? parentId, string name, string description)
        {
            int? parentCategoryId = parentId;
            string newCategoryName = name;
            string newCategoryDesc = description;
            Category newCategory = new Category();
            newCategory.Name = newCategoryName;
            newCategory.Description = newCategoryDesc;
            _context.Add(newCategory);
            await _context.SaveChangesAsync();

            CategoryCategory newCategoryToCategory = new CategoryCategory();
            newCategoryToCategory.CategoryId = newCategory.Id;
            newCategoryToCategory.ParentCategoryId = parentCategoryId;
            _context.Add(newCategoryToCategory);

            await _context.SaveChangesAsync();
        }

        public async Task AddTopLevelCategory(string name, string description)
        {
            Category newCategory = new Category();
            newCategory.Name = name;
            newCategory.Description = description;
            _context.Add(newCategory);
            await _context.SaveChangesAsync();

            CategoryCategory newCategoryToCategory = new CategoryCategory();
            newCategoryToCategory.CategoryId = newCategory.Id;
            newCategoryToCategory.ParentCategoryId = null;
            _context.Add(newCategoryToCategory);

            await _context.SaveChangesAsync();

        }

        public async Task<ProductCategory> GetProductCategoryById(int id)
        {
            return await _context.ProductCategory.FindAsync(id);
        }

        public async Task<IList<ProductCategory>> GetAllProductCategories()
        {
            return await _context.ProductCategory.ToListAsync();
        }

        public async Task UpdateProductCategory(ProductCategory productCategory)
        {
            _context.Update(productCategory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductCategory(int id)
        {
            _context.ProductCategory.Remove(await GetProductCategoryById(id));
            await _context.SaveChangesAsync();
        }

        public async Task RenameCategory(int Id, string rowVersion, string newName, string newDescription)
        {
            Category category = await GetCategoryById(Id);

            _context.Entry(category).Property("RowVersion").OriginalValue = Convert.FromBase64String(rowVersion);

            var test = Convert.FromBase64String(rowVersion);

            category.Name = newName;
            category.Description = newDescription;
            _context.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategory(int Id)
        {
            Category category = await GetCategoryById(Id);

            await DeleteSubcategories(category);// recursive method

            _context.Remove(category);

            ICollection<CategoryCategory> categoryCategories = await _context.CategoryCategory.Where(cc => cc.CategoryId == category.Id).ToListAsync();
            foreach (var cc in categoryCategories)
            {
                _context.Remove(cc);
            }

            await _context.SaveChangesAsync();
        }

        private async Task<int> DeleteSubcategories(Category category)
        {
            if (category != null) try
                {
                    ICollection<Category> subCategories = await GetChildCategoriesAsync(category);
                    foreach (var subCategory in subCategories)
                    {
                        if (await DeleteSubcategories(subCategory) == 0)// recursion
                        {
                            ICollection<CategoryCategory> categoryCategories = await _context.CategoryCategory.Where(cc => cc.CategoryId == subCategory.Id).ToListAsync();
                            foreach (var cc in categoryCategories)
                            {
                                _context.Remove(cc);
                            }
                            _context.Remove(subCategory);
                            await _context.SaveChangesAsync();
                        }
                        else return 1; //error
                    }
                    return 0; //has no more sub-categories: OK
                }
                catch (Exception)
                {
                    return 1; //error
                }
            else
                return 1;
        }
    }
}
