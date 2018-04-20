using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EShop.Models;
using EShop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using EShop.Business;

namespace EShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INavigationService _navigationService;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, INavigationService navigationService)
        {
            _context = context;
            _userManager = userManager;
            _navigationService = navigationService;
        }

        // GET, POST
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? categoryId, bool backToParentCategory, string absoluteNavigationPath, string navigateToCategoryNamed)
        {
            if (categoryId == null)
            {
                if (navigateToCategoryNamed == null)// (GET)
                {
                    ICollection<CategoryViewModel> categoryViewModels = await _navigationService.BuildRecursiveMenuAsync();
                    ViewBag.TopLevelCategories = categoryViewModels;
                    ViewBag.ParentCategoryId = null;
                    ViewBag.AbsoluteNavigationPath = null;

                    return View(await _context.Product.ToListAsync());
                }
                else// (POST) specific path segment selected ([segment]/.../...)
                {
                    // get that category by name
                    Category category = await (from c in _context.Category
                                                        where c.Name == navigateToCategoryNamed
                                                        select c).FirstAsync();

                    // build sub-categories
                    ICollection<CategoryViewModel> categoryViewModels = await _navigationService.BuildRecursiveSubMenuAsync(category);
                    // get category products
                    ICollection<Product> products = await _navigationService.GetProductsInCategoryAsync(category);

                    ViewBag.TopLevelCategories = categoryViewModels;

                    string[] pathSegments = absoluteNavigationPath.Split("/");
                    pathSegments = pathSegments.Skip(1).ToArray();// remove empty segment ([empty segment]/[some segment]/...)

                    int parentCategoryIndexInPath = Array.IndexOf(pathSegments, navigateToCategoryNamed);
                    string newAbsoluteNavigationPath = null;
                    string parentCategoryName = null;

                    await Task.Run(() =>
                    {
                        for (int i = 0; i < pathSegments.Count(); i++)
                        {
                            if (i == parentCategoryIndexInPath) parentCategoryName = pathSegments[i];
                            if (i <= parentCategoryIndexInPath) newAbsoluteNavigationPath += ("/" + pathSegments[i]);
                        }
                    });
                    Category parentCategory = await (from c in _context.Category
                                                        where c.Name == parentCategoryName
                                                        select c).FirstAsync();

                    ViewBag.ParentCategoryId = parentCategory.Id;
                    ViewBag.AbsoluteNavigationPath = newAbsoluteNavigationPath;
                    ViewBag.CurrentCategoryName = parentCategory.Name;

                    return View(products);
                }
            }
            else// (POST) backward and forward navigation
            {
                Category currentCategory = await _context.Category.FindAsync(categoryId);

                if (backToParentCategory)// Backward navigation
                {
                    absoluteNavigationPath = await _navigationService.RemoveLastUriSegmentAsync(absoluteNavigationPath);
                    ViewBag.AbsoluteNavigationPath = absoluteNavigationPath;

                    ICollection<CategoryViewModel> categoryViewModels = null;// category and category children
                    Category parentCategory = null;

                    // categories may have multiple parent-categories
                    ICollection<Category> parentCategories = await _navigationService.GetParentCategoriesAsync(currentCategory);

                    if (!parentCategories.Any())// already a top-level category
                    {
                        categoryViewModels = await _navigationService.BuildRecursiveMenuAsync();
                        ViewBag.ParentCategoryId = null;
                        ViewBag.TopLevelCategories = categoryViewModels;

                        return View(await _context.Product.ToListAsync());
                    }
                    else// not a top-level category
                    {
                        foreach (Category pCategory in parentCategories)
                        {
                            // search for parent-category based on absoluteNavigationPath last segment
                            if (pCategory.Name.Equals(absoluteNavigationPath.Split('/').Last()))
                            {
                                parentCategory = pCategory;
                                break;
                            }
                        }
                        categoryViewModels = await _navigationService.BuildRecursiveSubMenuAsync(parentCategory);
                        ViewBag.CurrentCategoryName = parentCategory.Name;
                        ViewBag.ParentCategoryId = parentCategory.Id;
                        ViewBag.TopLevelCategories = categoryViewModels;

                        return View(await _navigationService.GetProductsInCategoryAsync(parentCategory));
                    }
                }
                else // Forward navigation
                {
                    // Add new segment to absoluteNavigationPath
                    absoluteNavigationPath += ("/" + currentCategory.Name);

                    ICollection<CategoryViewModel> categoryViewModels = await _navigationService.BuildRecursiveSubMenuAsync(currentCategory);

                    ViewBag.TopLevelCategories = categoryViewModels;
                    ViewBag.ParentCategoryId = categoryId;
                    ViewBag.CurrentCategoryName = currentCategory.Name;
                    ViewBag.AbsoluteNavigationPath = absoluteNavigationPath;

                    if (categoryId == null)
                    {
                        return View(await _context.Product.ToListAsync());
                    }
                    else return View(await _navigationService.GetProductsInCategoryAsync(currentCategory));
                }
            }
        }

        //Denis added product page, not tested yet
        [AllowAnonymous]
        public async Task<IActionResult> ProductPage(int id)
        {
            Product temp = _context.Product.Where(p => p.Id == id).Single();
            ViewData["product_id"] = temp.Id;
            ViewData["product_name"] = temp.Name;
            ViewData["product_price"] = temp.Price;
            return View(await _context.ProductDetails.Where(p => p.ProductId == id).ToListAsync());
        }
    }
}
