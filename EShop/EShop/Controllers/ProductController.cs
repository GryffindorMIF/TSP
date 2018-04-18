using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EShop.Data;
using EShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(bool showAlert = false) //By default don't show alert about delete success
        {
            ViewData["show_alert"] = showAlert;
            return View(await _context.Product.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = new ProductCategoryViewModel();
            var categories = (from c in _context.Category
                              select c).ToList();

            model.CategoryMultiSelectList = new MultiSelectList(categories, "Id", "Name");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                await Task.Run(() =>
                {
                    Product product = new Product
                    {
                        Name = model.Product.Name,
                        Description = model.Product.Description,
                        Price = model.Product.Price
                    };
                    _context.Add(product);

                    foreach (int categoryId in model.SelectedCategoryIds)
                    {
                        ProductCategory productCategory = new ProductCategory
                        {
                            ProductId = product.Id,
                            CategoryId = categoryId
                        };
                        _context.Add(productCategory);
                    }
                });
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            var model = new ProductCategoryViewModel();

            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            IEnumerable<Category> categories = null;
            IEnumerable<int> selectedCategoryIds = null;

            var task = Task.Run(() =>
           {
               categories = (from c in _context.Category
                             select c).ToList();

               selectedCategoryIds = (from pc in _context.ProductCategory
                                      where pc.ProductId == product.Id
                                      select pc.CategoryId).ToList();
           });
            task.Wait();

            model.Product = product;
            model.CategoryMultiSelectList = new MultiSelectList(categories, "Id", "Name", selectedCategoryIds);

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductCategoryViewModel model)
        {
            if (id != model.Product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    IEnumerable<ProductCategory> relatedProductCategories = null;
                    var task = Task.Run(() =>
                   {
                       relatedProductCategories = (from pc in _context.ProductCategory
                                                   where pc.ProductId == model.Product.Id
                                                   select pc).ToList();
                   });
                    task.Wait();

                    foreach (var pc in relatedProductCategories)
                    {
                        _context.Remove(pc);
                    };

                    foreach (int categoryId in model.SelectedCategoryIds)
                    {
                        ProductCategory productCategory = new ProductCategory
                        {
                            ProductId = model.Product.Id,
                            CategoryId = categoryId
                        };
                        _context.Update(productCategory);
                    }

                    _context.Update(model.Product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(model.Product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model.Product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.SingleOrDefaultAsync(m => m.Id == id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { showAlert = true });
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }



        //Denis product description changes BELOW
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageProperties(int id, bool showAlert = false)
        {
            ViewData["product_Id"] = id; //To retrieve it in view
            ViewData["show_alert"] = showAlert;
            return View(await _context.ProductDetails.Where(p => p.ProductId == id).ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddProperty(int productId) //Add Property view
        {
            ViewData["product_Id"] = productId;
            return View();
        }

        //Add property action
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProperty(int productId, [Bind("Id,Property,Description,ProductId")] ProductDetails productDetails)
        {
            if (ModelState.IsValid)
            {
                //productDetails.ProductId = productId;
                _context.Add(productDetails);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageProperties), new { id = productId });
            }
            return View(productDetails);
        }

        /* //Delete product property page
         [Authorize(Roles = "Admin")]
         public async Task<IActionResult> DeleteProperty(int? propertyId)
         {

             if (propertyId == null)
             {
                 return NotFound();
             }

             var property = await _context.ProductDetails.SingleOrDefaultAsync(p => p.Id == propertyId);

             if (property == null)
             {
                 return NotFound();
             }

             return View(property);
         }

         //Method to delete property
         [HttpPost, ActionName("Delete")]
         [Authorize(Roles = "Admin")]
         [ValidateAntiForgeryToken]
         public async Task<IActionResult> DeletePropertyConfirmed(int propertyId)
         {
             ProductDetails property = await _context.ProductDetails.SingleOrDefaultAsync(p => p.Id == propertyId);

             if (property == null)
             {
                 return NotFound();
             }

             int productId = property.ProductId;
             //var productDetail = await _context.ProductDetails.SingleOrDefaultAsync(pd => pd.Id == productId);
             _context.ProductDetails.Remove(property);
             await _context.SaveChangesAsync();
             return RedirectToAction(nameof(ManageProperties), new { id = productId }); //showAlert = true });
         }*/
    }
}