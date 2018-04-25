using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EShop.Data;
using EShop.Models;
using EShop.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _appEnvironment;

        public ProductController(ApplicationDbContext context, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
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

                    if (model.PrimaryImage != null)
                    {
                        var primaryImagePath = _appEnvironment.UploadImage(model.PrimaryImage);
                        ProductImage primaryImage = new ProductImage
                        {
                            IsPrimary = true,
                            ImageUrl = primaryImagePath,
                            Product = product
                        };
                        _context.Add(primaryImage);
                    }

                    if (model.OtherImages != null)
                    {
                        foreach (IFormFile image in model.OtherImages)
                        {
                            var otherImagePath = _appEnvironment.UploadImage(image);
                            ProductImage otherImage = new ProductImage
                            {
                                IsPrimary = false,
                                ImageUrl = otherImagePath,
                                Product = product
                            };
                            _context.Add(otherImage);
                        }
                    }

                    if (model.IdsOfSelectedCategories != null)
                    {
                        foreach (int categoryId in model.IdsOfSelectedCategories)
                        {
                            ProductCategory productCategory = new ProductCategory
                            {
                                ProductId = product.Id,
                                CategoryId = categoryId
                            };
                            _context.Add(productCategory);
                        }
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
            IEnumerable<ProductImage> images = null;

            var task = Task.Run(() =>
           {
               categories = (from c in _context.Category
                             select c).ToList();

               selectedCategoryIds = (from pc in _context.ProductCategory
                                      where pc.ProductId == product.Id
                                      select pc.CategoryId).ToList();

               images = (from i in _context.ProductImage
                         where i.Product == product
                         where i.IsPrimary == false
                         select i).ToList();
           });
            task.Wait();

            model.Product = product;
            model.CategoryMultiSelectList = new MultiSelectList(categories, "Id", "Name", selectedCategoryIds);
            model.ImagesToRemoveSelectList = new MultiSelectList(images, "Id", "ImageUrl");

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
                    List<ProductImage> possiblePrimaryImages = null;
                    List<ProductImage> possibleOtherImages = null;
                    var task = Task.Run(() =>
                   {
                       relatedProductCategories = (from pc in _context.ProductCategory
                                                   where pc.ProductId == model.Product.Id
                                                   select pc).ToList();

                       if (model.PrimaryImage != null)
                       {
                           possiblePrimaryImages = (from pi in _context.ProductImage
                                                   where pi.Product == model.Product
                                                   where pi.IsPrimary == true
                                                   select pi).ToList();
                       }

                       if (model.IdsOfSelectedImages != null)
                       {
                           //TODO: Select the selected
                           possibleOtherImages = (from oi in _context.ProductImage
                                                  where oi.Product == model.Product
                                                  select oi).ToList();
                       }
                   });
                    task.Wait();

                    //New primary image
                    if (possiblePrimaryImages != null && possiblePrimaryImages.Count > 0)
                    {
                        var primaryImagePath = _appEnvironment.UploadImage(model.PrimaryImage);
                        ProductImage primaryImage = new ProductImage
                        {
                            IsPrimary = true,
                            ImageUrl = primaryImagePath,
                            Product = model.Product
                        };
                        _appEnvironment.DeleteImage(possiblePrimaryImages[0].ImageUrl);
                        _context.Remove(possiblePrimaryImages[0]);
                        _context.Add(primaryImage);
                    }

                    //New additional images
                    if (model.OtherImages != null)
                    {
                        foreach (IFormFile image in model.OtherImages)
                        {
                            var otherImagePath = _appEnvironment.UploadImage(image);
                            ProductImage otherImage = new ProductImage
                            {
                                IsPrimary = false,
                                ImageUrl = otherImagePath,
                                Product = model.Product
                            };
                            _context.Add(otherImage);
                        }
                    }

                    //Remove old images
                    if (possibleOtherImages != null && possibleOtherImages.Count > 0)
                    {
                        foreach (ProductImage image in possibleOtherImages)
                        {
                            _appEnvironment.DeleteImage(image.ImageUrl);
                            _context.Remove(image);
                        }
                    }


                    foreach (var pc in relatedProductCategories)
                    {
                        _context.Remove(pc);
                    };

                    if (model.IdsOfSelectedCategories != null)
                    {
                        foreach (int categoryId in model.IdsOfSelectedCategories)
                        {
                            ProductCategory productCategory = new ProductCategory
                            {
                                ProductId = model.Product.Id,
                                CategoryId = categoryId
                            };
                            _context.Update(productCategory);
                        }
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
            List<ProductImage> images = null; 
            await Task.Run(() =>
            {
                images = (from oi in _context.ProductImage
                          where oi.Product == product
                          select oi).ToList();
            });

            //Remove images
            if (images != null && images.Count > 0)
            {
                foreach (ProductImage image in images)
                {
                    _appEnvironment.DeleteImage(image.ImageUrl);
                    _context.Remove(image);
                }
            }


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
            ViewData["product_id"] = id; //To retrieve it in view
            ViewData["show_alert"] = showAlert;
            return View(await _context.ProductDetails.Where(p => p.ProductId == id).ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AddProperty(int productId) //Add Property view
        {
            ViewData["product_id"] = productId;
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