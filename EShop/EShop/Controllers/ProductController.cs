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
using Microsoft.Extensions.Configuration;

namespace EShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly int uploadMaxByteSize;

        public ProductController(ApplicationDbContext context, IHostingEnvironment appEnvironment, IConfiguration configuration)
        {
            _context = context;
            _appEnvironment = appEnvironment;
            if (!int.TryParse(configuration["FileManagerConfig:UploadMaxByteSize"], out uploadMaxByteSize))
            {
                throw new InvalidOperationException("Invalid FileManagerConfig:UploadMaxByteSize in appsettings.json. Not an int value.");
            }

        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(bool showAlert = false) //By default don't show alert about delete success
        {
            ViewData["show_alert"] = showAlert;
            return View(await _context.Product.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = new ProductCategoryViewModel();
            var categories = (from c in _context.Category
                              select c).ToList();

            model.CategoryMultiSelectList = new MultiSelectList(categories, "Id", "Name");

            ViewBag.UploadMaxMbSize = uploadMaxByteSize / 1048576;
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
                        var primaryImagePath = _appEnvironment.UploadImageAsync(model.PrimaryImage, uploadMaxByteSize).GetAwaiter().GetResult();
                        if (primaryImagePath != null)
                        {
                            ProductImage primaryImage = new ProductImage
                            {
                                IsPrimary = true,
                                ImageUrl = primaryImagePath,
                                Product = product
                            };
                            _context.Add(primaryImage);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Primary image does not match the file requirements.");
                            return;
                        }
                    }

                    if (model.OtherImages != null)
                    {
                        foreach (IFormFile image in model.OtherImages)
                        {
                            var otherImagePath = _appEnvironment.UploadImageAsync(image, uploadMaxByteSize).GetAwaiter().GetResult(); ;
                            if (otherImagePath != null)
                            {
                                ProductImage otherImage = new ProductImage
                                {
                                    IsPrimary = false,
                                    ImageUrl = otherImagePath,
                                    Product = product
                                };
                                _context.Add(otherImage);
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "One of the additional image files doesn't match the file requirements.");
                                return;
                            }
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
                if (!ModelState.IsValid)
                {
                    ViewBag.UploadMaxMbSize = uploadMaxByteSize / 1048576;
                    return View(model);
                }
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
            List<ProductImage> otherImages = null;
            List<ProductImage> primaryImages = null;

            var task = Task.Run(() =>
           {
               categories = (from c in _context.Category
                             select c).ToList();

               selectedCategoryIds = (from pc in _context.ProductCategory
                                      where pc.ProductId == product.Id
                                      select pc.CategoryId).ToList();

               otherImages = (from i in _context.ProductImage
                              where i.Product == product
                              where i.IsPrimary == false
                              select i).ToList();

               primaryImages = (from pi in _context.ProductImage
                                where pi.Product == product
                                where pi.IsPrimary == true
                                select pi).ToList();
           });
            task.Wait();

            model.Product = product;
            model.CategoryMultiSelectList = new MultiSelectList(categories, "Id", "Name", selectedCategoryIds);
            model.ImagesToRemoveSelectList = new MultiSelectList(otherImages, "Id", "ImageUrl");

            if (primaryImages.Count > 0)
            {
                ViewBag.PrimaryImage = primaryImages[0].ImageUrl;
            }
            else
            {
                ViewBag.PrimaryImage = "product-image-placeholder.jpg";
            }
            ViewBag.OtherImages = otherImages;
            ViewBag.UploadMaxMbSize = uploadMaxByteSize / 1048576;

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
                           possibleOtherImages = (from oi in _context.ProductImage
                                                  where oi.Product == model.Product
                                                  where oi.IsPrimary == false
                                                  select oi).ToList();
                       }
                   });
                    task.Wait();

                    //New primary image
                    if (model.PrimaryImage != null)
                    {
                        var primaryImagePath = await _appEnvironment.UploadImageAsync(model.PrimaryImage, uploadMaxByteSize);
                        if (primaryImagePath != null)
                        {
                            ProductImage primaryImage = new ProductImage
                            {
                                IsPrimary = true,
                                ImageUrl = primaryImagePath,
                                Product = model.Product
                            };
                            if (possiblePrimaryImages != null && possiblePrimaryImages.Count > 0)
                            {
                                await _appEnvironment.DeleteImageAsync(possiblePrimaryImages[0].ImageUrl);
                                _context.Remove(possiblePrimaryImages[0]);
                            }
                            _context.Add(primaryImage);
                        }
                        // TODO: Change from silently failing to failing normally without bugging out the edit view (no relation based data populated)
                        /*else
                        {
                            ModelState.AddModelError(string.Empty, "Primary image file is not an image.");
                            return View(model);
                        }*/
                    }

                    //New additional images
                    if (model.OtherImages != null)
                    {
                        foreach (IFormFile image in model.OtherImages)
                        {
                            var otherImagePath = await _appEnvironment.UploadImageAsync(image, uploadMaxByteSize);
                            if (otherImagePath != null)
                            {
                                ProductImage otherImage = new ProductImage
                                {
                                    IsPrimary = false,
                                    ImageUrl = otherImagePath,
                                    Product = model.Product
                                };
                                _context.Add(otherImage);
                            }
                            // TODO: Change from silently failing to failing normally without bugging out the edit view (no relation based data populated)
                            /*else
                            {
                                ModelState.AddModelError(string.Empty, "One of the additional image files is not an image.");
                                return View(model);
                            }*/
                        }
                    }

                    //Remove old images
                    if (possibleOtherImages != null && possibleOtherImages.Count > 0 && model.IdsOfSelectedImages != null)
                    {

                        foreach (int imageId in model.IdsOfSelectedImages)
                        {
                            List<ProductImage> imageToClean = possibleOtherImages.Where(image => image.Id == imageId).ToList();
                            if (imageToClean.Count > 0)
                            {
                                await _appEnvironment.DeleteImageAsync(imageToClean[0].ImageUrl);
                                _context.Remove(imageToClean[0]);
                            }
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
                    await _appEnvironment.DeleteImageAsync(image.ImageUrl);
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



        //Product properties management below
        //Page with all product properties with "delete" button
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageProperties(int id, bool showAlert = false)
        {
            Product temp = _context.Product.First(p => p.Id == id);
            //Using ViewData to retrieve values in view
            ViewData["product_name"] = temp.Name;
            ViewData["product_id"] = id;
            //ViewData["show_alert"] = showAlert;
            return View(await _context.ProductDetails.Where(p => p.ProductId == id).ToListAsync());
        }

        //Page with add property form
        [Authorize(Roles = "Admin")]
        public IActionResult AddProperty(int productId) //Add Property view
        {
            Product temp = _context.Product.First(p => p.Id == productId);
            ViewData["product_name"] = temp.Name;
            ViewData["product_id"] = temp.Id;
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
            //If admin did not fill every field then refresh page
            Product temp = _context.Product.First(p => p.Id == productId);
            ViewData["product_name"] = temp.Name;
            ViewData["product_id"] = temp.Id;
            return View();
        }

        //Remove product property delete button action
        public async Task<IActionResult> RemoveProductProperty(int id)
        {
            ProductDetails property = _context.ProductDetails.FirstOrDefault(pd => pd.Id == id);
            int productId = property.ProductId;

            await Task.Run(() =>
            {
                try
                {
                    Product product = _context.Product.FirstOrDefault(p => p.Id == property.ProductId);
                    _context.Remove(property);
                    //_context.Update(product);

                    var t2 = Task.Run(
                        async () =>
                        {
                            await _context.SaveChangesAsync();
                        });
                    t2.Wait();
                    return RedirectToAction(nameof(ManageProperties), new { id = productId });
                }
                catch (Exception)
                {
                    return RedirectToAction(nameof(Index));
                }
            });
            return RedirectToAction(nameof(ManageProperties), new { id = productId });
        }
    }
}