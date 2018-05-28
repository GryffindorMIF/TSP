using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShop.Business.Interfaces;
using EShop.Data;
using EShop.Models.EFModels.Category;
using EShop.Models.EFModels.Product;
using EShop.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EShop.Business.Services
{
    public class NavigationService : INavigationService
    {
        private readonly ApplicationDbContext _context;

        public NavigationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddSubCategory(int? parentCatId, int childCatId)
        {
            try
            {
                var cc = new CategoryCategory
                {
                    Category = await _context.Category.FindAsync(childCatId),
                    ParentCategory = parentCatId == null ? null : await _context.Category.FindAsync(parentCatId)
                };
                if (parentCatId != null && await _context.CategoryCategory.CountAsync(catc => catc.CategoryId == cc.Category.Id && catc.ParentCategoryId == cc.ParentCategory.Id) != 0) throw new Exception();
                if (parentCatId == null && await _context.CategoryCategory.CountAsync(catc => catc.CategoryId == cc.Category.Id && catc.ParentCategoryId == null) != 0) throw new Exception();

                _context.Add(cc);
                await _context.SaveChangesAsync();
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
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

        public async Task<ICollection<Product>> GetProductsInCategoryByPageAsync(Category category, int pageNumber,
            int productsPerPage)
        {
            ICollection<Product> productsInCategory = null;

            await Task.Run(() =>
            {
                if (category != null)
                    productsInCategory = (from p in _context.Product
                        join pc in _context.ProductCategory on p.Id equals pc.ProductId
                        where pc.CategoryId == category.Id
                        select p).Skip(pageNumber * productsPerPage).Take(productsPerPage).ToList();
                else
                    productsInCategory = _context.Product.Skip(pageNumber * productsPerPage).Take(productsPerPage)
                        .ToList();
            });

            return productsInCategory;
        }

        public async Task<ICollection<Product>> GetProductsInCategoryByPageAsync(Category category, int pageNumber,
            int productsPerPage, string attributeName)
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
            var pageCount = 0;

            await Task.Run(() =>
            {
                if (category != null)
                    productsTotalCount = (from p in _context.Product
                        join pc in _context.ProductCategory on p.Id equals pc.ProductId
                        where pc.CategoryId == category.Id
                        select p).Count();
                else
                    productsTotalCount = _context.Product.Count();

                pageCount = productsTotalCount / productsPerPage;

                if (productsTotalCount % productsPerPage != 0) pageCount++;
            });
            return pageCount;
        }

        public async Task<int> GetProductsInCategoryPageCount(Category category, int productsPerPage,
            string attributeName)
        {
            int productsTotalCount;
            var pageCount = 0;

            await Task.Run(() =>
            {
                if (category != null)
                    productsTotalCount = (from p in _context.Product
                        from a in _context.AttributeValue
                        join pc in _context.ProductCategory on p.Id equals pc.ProductId
                        join pa in _context.ProductAttributeValue on p.Id equals pa.ProductId
                        where pc.CategoryId == category.Id
                        where pa.AttributeValueId == a.Id
                        where a.Name == attributeName
                        select p).Count();
                else
                    productsTotalCount = _context.Product.Count();

                pageCount = productsTotalCount / productsPerPage;

                if (productsTotalCount % productsPerPage != 0) pageCount++;
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
                return await GetTopLevelCategoriesAsync();
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
            var topLevelCategories = await GetTopLevelCategoriesAsync();
            ICollection<CategoryViewModel> categoryViewModels = new List<CategoryViewModel>();

            foreach (var topLevelCategory in topLevelCategories)
            {
                var categoryViewModel = new CategoryViewModel
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
            var childCategories = await GetChildCategoriesAsync(parentCategory);
            ICollection<CategoryViewModel> categoryViewModels = new List<CategoryViewModel>();

            if (childCategories.Count > 0)
                foreach (var childCategory in childCategories)
                {
                    var categoryViewModel = new CategoryViewModel
                    {
                        Category = childCategory,
                        SubCategories = await BuildRecursiveSubMenuAsync(childCategory) // recursion
                    };
                    categoryViewModels.Add(categoryViewModel);
                }

            return categoryViewModels;
        }

        public async Task<ICollection<Category>> GetParentCategoriesAsync(Category childCategory)
        {
            ICollection<int?> parentCategoriesIds = await (from cc in _context.CategoryCategory
                where cc.CategoryId == childCategory.Id
                select cc.ParentCategoryId).ToListAsync();

            ICollection<Category> parentCategories = new List<Category>();

            foreach (var parentCategoryId in parentCategoriesIds)
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
                var pathSegments = uri.Split("/");
                uri = "";
                for (var i = 0; i < pathSegments.Length - 1; i++)
                    if (i == 0)
                        uri += pathSegments[i];
                    else
                        uri += "/" + pathSegments[i];
            });
            return uri;
        }

        public async Task<Category> GetCategoryById(int? id)
        {
            var category = await _context.Category.FindAsync(id);
            return category;
        }

        public async Task<Category> GetCategoryByName(string name)
        {
            try
            {
                var category = await (from c in _context.Category
                    where c.Name == name
                    select c).FirstAsync(); // category name is unique
                return category;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task AddCategory(int? parentId, string name, string description)
        {
            var parentCategoryId = parentId;
            var newCategoryName = name;
            var newCategoryDesc = description;
            var newCategory = new Category
            {
                Name = newCategoryName,
                Description = newCategoryDesc
            };
            _context.Add(newCategory);
            await _context.SaveChangesAsync();

            var newCategoryToCategory = new CategoryCategory
            {
                CategoryId = newCategory.Id,
                ParentCategoryId = parentCategoryId
            };
            _context.Add(newCategoryToCategory);

            await _context.SaveChangesAsync();
        }

        public async Task AddTopLevelCategory(string name, string description)
        {
            var newCategory = new Category
            {
                Name = name,
                Description = description
            };
            _context.Add(newCategory);
            await _context.SaveChangesAsync();

            var newCategoryToCategory = new CategoryCategory
            {
                CategoryId = newCategory.Id,
                ParentCategoryId = null
            };
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

        public async Task RenameCategory(int id, string rowVersion, string newName, string newDescription)
        {
            var category = await GetCategoryById(id);

            _context.Entry(category).Property("RowVersion").OriginalValue = Convert.FromBase64String(rowVersion);

            category.Name = newName;
            category.Description = newDescription;
            _context.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task RenameCategory(int id, string newName, string newDescription)
        {
            var category = await GetCategoryById(id);

            category.Name = newName;
            category.Description = newDescription;
            _context.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategory(int categoryId, bool cascade, bool refOnly, int? parentCategoryId)
        {
            try
            {
                var category = await GetCategoryById(categoryId);
                await DeleteSubcategories(category, cascade); // recursive method

                if(!refOnly) _context.Remove(category);

                if (cascade)
                {
                    ICollection<CategoryCategory> categoryCategories =
                        await _context.CategoryCategory.Where(cc => cc.CategoryId == category.Id).ToListAsync();
                    foreach (var cc in categoryCategories) _context.Remove(cc);
                }
                else if (refOnly)
                {
                    var cc = await _context.CategoryCategory.FirstOrDefaultAsync(catC =>
                        catC.ParentCategory.Id == parentCategoryId && catC.Category.Id == categoryId);
                    _context.Remove(cc);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                //Ignored
            }
        }

        private async Task<int> DeleteSubcategories(Category category, bool cascade)
        {
            if (category != null)
                try
                {
                    ICollection<Category> subCategories = await GetChildCategoriesAsync(category);
                    foreach (var subCategory in subCategories)
                        if (await DeleteSubcategories(subCategory, cascade) == 0) // recursion
                        {
                            ICollection<CategoryCategory> categoryCategories = await _context.CategoryCategory
                                .Where(cc => cc.CategoryId == subCategory.Id).ToListAsync();
                            foreach (var cc in categoryCategories) _context.Remove(cc);
                            if(cascade) _context.Remove(subCategory);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            return 1; //error
                        }

                    return 0; //has no more sub-categories: OK
                }
                catch (Exception)
                {
                    return 1; //error
                }

            return 1;
        }
    }
}