using System;
using System.Threading.Tasks;
using EShop.Business.Interfaces;
using EShop.Models.PostModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShop.Controllers
{
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class CategoryController : Controller
    {
        private readonly INavigationService _navigationService;

        public CategoryController(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
        /*
        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var recursiveMenu = await _navigationService.BuildRecursiveMenuAsync();
            return View(recursiveMenu);

            //return View(await _context.Category.ToListAsync());
        }
        */
        [HttpPost]
        public async Task<IActionResult> RenameCategory([FromBody] CategoryNewNamePostModel postModel)
        {
            try
            {
                await _navigationService.GetCategoryById(postModel.CategoryId);

                if (postModel.RowVersion == null)
                    await _navigationService.RenameCategory(postModel.CategoryId, postModel.NewName,
                        postModel.NewDescription);
                else
                    await _navigationService.RenameCategory(postModel.CategoryId, postModel.RowVersion,
                        postModel.NewName, postModel.NewDescription);


                return Json(0); // success
            }
            catch (DbUpdateConcurrencyException)
            {
                return Json(2); // Optimistic locking
            }
            catch
            {
                return Json(1); // exception
            }
        }

        // AJAX
        [HttpPost]
        public async Task<IActionResult> DeleteCategory([FromBody] CategoryPostModel postModel)
        {
            try
            {
                await _navigationService.DeleteCategory(postModel.CategoryId, postModel.Cascade, postModel.ReferenceOnly, postModel.ParentCategoryId);

                return Json(0); // success
            }
            catch
            {
                return Json(1); // exception
            }
        }

        // AJAX
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryPostModel postModel)
        {
            try
            {
                if (postModel.ExistingCategoryId == null)
                {
                    await _navigationService.AddCategory(postModel.ParentCategoryId, postModel.CategoryName, postModel.CategoryDescription);
                }
                else
                {
                    if(await _navigationService.AddSubCategory(postModel.ParentCategoryId, (int) postModel.ExistingCategoryId) == 1)
                        throw new Exception();
                }
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
            return RedirectToAction("Index", "Home"); //success
        }
    }
}