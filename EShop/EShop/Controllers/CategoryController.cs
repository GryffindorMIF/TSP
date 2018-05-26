using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EShop.Models;
using EShop.Business;
using Microsoft.AspNetCore.Authorization;

namespace EShop.Views
{
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class CategoryController : Controller
    {
        private readonly INavigationService _navigationService;

        public CategoryController(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            ICollection<CategoryViewModel> recursiveMenu = await _navigationService.BuildRecursiveMenuAsync();
            return View(recursiveMenu);

            //return View(await _context.Category.ToListAsync());
        }

        // AJAX
        [HttpPost]
        public async Task<IActionResult> RenameCategory([FromBody] CategoryNewNamePostModel postModel)
        {
            try
            {
                Category category = await _navigationService.GetCategoryById(postModel.CategoryId);

                if (postModel.RowVersion == null)
                {
                    await _navigationService.RenameCategory(postModel.CategoryId, postModel.NewName, postModel.NewDescription);
                }
                else
                {
                    await _navigationService.RenameCategory(postModel.CategoryId, postModel.RowVersion, postModel.NewName, postModel.NewDescription);
                }


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
        public async Task<IActionResult> DeleteCategory([FromBody] CategoryPostModel postModel)
        {
            try
            {
                await _navigationService.DeleteCategory(postModel.CategoryId);

                return Json(0);// success
            }
            catch
            {
                return Json(1);// exception
            }
        }

        // AJAX
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryPostModel postModel)
        {
            try
            {
                await _navigationService.AddCategory(postModel.ParentCategoryId, postModel.CategoryName, postModel.CategoryDescription);

                return Json(0); //success
            }
            catch
            {
    
                return Json(1); //exception
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddTopLevelCategory(string name, string description)
        {
            await _navigationService.AddTopLevelCategory(name, description);
            return RedirectToAction("Index", "Home");//success
        }
    }
}
