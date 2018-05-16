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

        public async Task<List<Category>> GetChildCategoriesAsync(Category parentCategory)
        {

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
    }
}
