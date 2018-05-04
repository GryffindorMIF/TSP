using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EShop.Data;
using EShop.Models;
using EShop.Business;
using Microsoft.AspNetCore.Authorization;

namespace EShop.Views
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INavigationService _navigationService;

        public CategoryController(ApplicationDbContext context, INavigationService navigationService)
        {
            _context = context;
            _navigationService = navigationService;
        }

        // GET: Categories
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            ICollection<CategoryViewModel> recursiveMenu = await _navigationService.BuildRecursiveMenuAsync();
            return View(recursiveMenu);

            //return View(await _context.Category.ToListAsync());
        }

        // AJAX
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RenameCategory([FromBody] CategoryNewNamePostModel postModel)
        {
            try
            {
                Category category = await _context.Category.FindAsync(postModel.CategoryId);

                _context.Entry(category).Property("RowVersion").OriginalValue = Convert.FromBase64String(postModel.RowVersion);

                var test = Convert.FromBase64String(postModel.RowVersion);

                category.Name = postModel.NewName;
                category.Description = postModel.NewDescription;
                _context.Update(category);
                await _context.SaveChangesAsync();

                return Json(0);// success
            }
            catch (DbUpdateConcurrencyException)
            {
                return Json(2); // Optimistic locking
            }
            catch
            {
                return Json(1);// exception
            }
        }

        // AJAX
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory([FromBody] CategoryPostModel postModel)
        {
            try
            {
                Category category = await _context.Category.FindAsync(postModel.CategoryId);

                await DeleteSubcategories(category);// recursive method

                _context.Remove(category);

                ICollection<CategoryCategory> categoryCategories = await _context.CategoryCategory.Where(cc => cc.CategoryId == category.Id).ToListAsync();
                foreach(var cc in categoryCategories)
                {
                    _context.Remove(cc);
                }

                await _context.SaveChangesAsync();

                return Json(0);// success
            }
            catch
            {
                return Json(1);// exception
            }
        }

        private async Task<int> DeleteSubcategories(Category category)
        {
            try
            {
                ICollection<Category> subCategories = await _navigationService.GetChildCategoriesAsync(category);
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
                return 0;// has no more sub-categories: OK
            }
            catch
            {
                return 1;// error
            }
        }

        // AJAX
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryPostModel postModel)
        {
            int? parentCategoryId = postModel.ParentCategoryId;
            string newCategoryName = postModel.CategoryName;
            string newCategoryDesc = postModel.CategoryDescription;

            try
            {
                Category newCategory = new Category();
                newCategory.Name = newCategoryName;
                newCategory.Description = newCategoryDesc;
                _context.Add(newCategory);
                await _context.SaveChangesAsync();// nes reikes database sugeneruoto Id

                CategoryCategory newCategoryToCategory = new CategoryCategory();
                newCategoryToCategory.CategoryId = newCategory.Id;
                newCategoryToCategory.ParentCategoryId = parentCategoryId;
                _context.Add(newCategoryToCategory);

                await _context.SaveChangesAsync();

                return Json(0);//success
            }
            catch
            {
                return Json(1);// exception
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTopLevelCategory(string name, string description)
        {
            Category newCategory = new Category();
            newCategory.Name = name;
            newCategory.Description = description;
            _context.Add(newCategory);
            await _context.SaveChangesAsync();// nes reikes database sugeneruoto Id

            CategoryCategory newCategoryToCategory = new CategoryCategory();
            newCategoryToCategory.CategoryId = newCategory.Id;
            newCategoryToCategory.ParentCategoryId = null;
            _context.Add(newCategoryToCategory);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");//success
        }
    }
}
