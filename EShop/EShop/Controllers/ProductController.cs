﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShop.Business.Interfaces;
using EShop.Models.EFModels.Product;
using EShop.Models.ViewModels.Product;
using EShop.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EShop.Controllers
{
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class ProductController : Controller
    {
        private readonly IHostingEnvironment _appEnvironment;
        private readonly INavigationService _navigationService;
        private readonly IProductService _productService;
        private readonly int _uploadMaxByteSize;

        public ProductController(IHostingEnvironment appEnvironment, IConfiguration configuration,
            IProductService productService, INavigationService navigationService)
        {
            _appEnvironment = appEnvironment;
            _productService = productService;
            _navigationService = navigationService;
            if (!int.TryParse(configuration["FileManagerConfig:UploadMaxByteSize"], out _uploadMaxByteSize))
                throw new InvalidOperationException(
                    "Invalid FileManagerConfig:UploadMaxByteSize in appsettings.json. Not an int value.");
        }

        public IActionResult Index() //By default don't show alert about delete success
        {
            //ViewData["show_alert"] = showAlert;
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Create()
        {
            var model = new ProductCategoryViewModel();
            var categories = await _navigationService.GetAllCategories();

            model.CategoryMultiSelectList = new MultiSelectList(categories, "Id", "Name");

            ViewBag.UploadMaxMbSize = _uploadMaxByteSize / 1048576;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                await Task.Run(async () =>
                {
                    var product = new Product
                    {
                        Name = model.Product.Name,
                        Description = model.Product.Description,
                        Price = model.Product.Price
                    };
                    //_context.Add(product);
                    var result = await _productService.CreateProduct(product);

                    if (result == 0)
                    {
                        if (model.PrimaryImage != null)
                        {
                            var primaryImagePath = _appEnvironment
                                .UploadImageAsync(model.PrimaryImage, "products", _uploadMaxByteSize).GetAwaiter()
                                .GetResult();
                            if (primaryImagePath != null)
                            {
                                var primaryImage = new ProductImage
                                {
                                    IsPrimary = true,
                                    ImageUrl = primaryImagePath,
                                    Product = product
                                };
                                //_context.Add(primaryImage);
                                await _productService.AddProductImage(primaryImage);
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty,
                                    "Primary image does not match the file requirements.");
                                return;
                            }
                        }

                        if (model.OtherImages != null)
                            foreach (var image in model.OtherImages)
                            {
                                var otherImagePath = _appEnvironment
                                    .UploadImageAsync(image, "products", _uploadMaxByteSize).GetAwaiter().GetResult();
                                if (otherImagePath != null)
                                {
                                    var otherImage = new ProductImage
                                    {
                                        IsPrimary = false,
                                        ImageUrl = otherImagePath,
                                        Product = product
                                    };
                                    //_context.Add(otherImage);
                                    await _productService.AddProductImage(otherImage);
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty,
                                        "One of the additional image files doesn't match the file requirements.");
                                    return;
                                }
                            }

                        if (model.IdsOfSelectedCategories != null)
                            foreach (var categoryId in model.IdsOfSelectedCategories)
                            {
                                var productCategory = new ProductCategory
                                {
                                    ProductId = product.Id,
                                    CategoryId = categoryId
                                };
                                //_context.Add(productCategory);
                                await _productService.AddProductToCategory(productCategory);
                            }

                        TempData["SuccessMsg"] = "Product created successfully.";
                    }
                    else // some constraints violated
                    {
                        TempData["ErrorMsg"] = "Product creation failed: Unique name constraint violated.";
                    }
                });
                if (!ModelState.IsValid)
                {
                    ViewBag.UploadMaxMbSize = _uploadMaxByteSize / 1048576;
                    return View(model);
                }

                //await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var model = new ProductCategoryViewModel();

            if (id == null)
            {
                TempData["ErrorMsg"] = "Product has been deleted by another staff member";
                return RedirectToAction("Index", "Home");
            }


            var product = await _productService.FindProductByIdAsync((int) id);
            if (product == null)
            {
                TempData["ErrorMsg"] = "Product has been deleted by another staff member";
                return RedirectToAction("Index", "Home");
            }

            model = await FillUpProductEditData(model, product);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductCategoryViewModel model)
        {
            if (id != model.Product.Id)
            {
                TempData["ErrorMsg"] = "Product has been deleted by another staff member";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    IList<ProductImage> possiblePrimaryImages = null;
                    IList<ProductImage> possibleOtherImages = null;
                    var relatedProductCategories = await _navigationService.GetProductCategories(model.Product.Id);

                    if (model.PrimaryImage != null)
                        possiblePrimaryImages = await _productService.GetProductImages(model.Product.Id);

                    if (model.IdsOfSelectedImages != null)
                        possibleOtherImages = await _productService.GetProductImages(model.Product.Id, false);

                    //New primary image
                    if (model.PrimaryImage != null)
                    {
                        var primaryImagePath =
                            await _appEnvironment.UploadImageAsync(model.PrimaryImage, "products", _uploadMaxByteSize);
                        if (primaryImagePath != null)
                        {
                            var primaryImage = new ProductImage
                            {
                                IsPrimary = true,
                                ImageUrl = primaryImagePath,
                                Product = model.Product
                            };
                            if (possiblePrimaryImages != null && possiblePrimaryImages.Count > 0)
                            {
                                await _appEnvironment.DeleteImageAsync(possiblePrimaryImages[0].ImageUrl, "products");
                                await _productService.DeleteProductImage(possiblePrimaryImages[0]);
                            }

                            await _productService.AddProductImage(primaryImage);
                        }
                        else
                        {
                            model = await FillUpProductEditData(model, model.Product);
                            ModelState.AddModelError(string.Empty,
                                "Primary image file is of the wrong format or too large.");
                            return View(model);
                        }
                    }

                    //New additional images
                    if (model.OtherImages != null)
                        foreach (var image in model.OtherImages)
                        {
                            var otherImagePath =
                                await _appEnvironment.UploadImageAsync(image, "products", _uploadMaxByteSize);
                            if (otherImagePath != null)
                            {
                                var otherImage = new ProductImage
                                {
                                    IsPrimary = false,
                                    ImageUrl = otherImagePath,
                                    Product = model.Product
                                };
                                await _productService.AddProductImage(otherImage);
                            }
                            else
                            {
                                model = await FillUpProductEditData(model, model.Product);
                                ModelState.AddModelError(string.Empty,
                                    "One of the additional image files is of the wrong format or too large.");
                                return View(model);
                            }
                        }

                    //Remove old images
                    if (possibleOtherImages != null && possibleOtherImages.Count > 0 &&
                        model.IdsOfSelectedImages != null)
                        foreach (var imageId in model.IdsOfSelectedImages)
                        {
                            var imageToClean = possibleOtherImages.Where(image => image.Id == imageId).ToList();
                            if (imageToClean.Count > 0)
                            {
                                await _appEnvironment.DeleteImageAsync(imageToClean[0].ImageUrl, "products");
                                await _productService.DeleteProductImage(imageToClean[0]);
                            }
                        }

                    //Opt locking separated too. Leave it or keep it?
                    await _productService.UpdateRowVersionEntry(model.Product);

                    if (model.IdsOfSelectedCategories != null)
                        foreach (var categoryId in model.IdsOfSelectedCategories)
                        {
                            var productCategory = new ProductCategory
                            {
                                ProductId = model.Product.Id,
                                CategoryId = categoryId
                            };
                            await _navigationService.UpdateProductCategory(productCategory);
                        }

                    foreach (var pc in relatedProductCategories) await _navigationService.DeleteProductCategory(pc.Id);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if ((await _productService.GetAllProducts()).Any(p => p.Id == model.Product.Id) == false)
                    {
                        TempData["ErrorMsg"] = "Product has been deleted by another staff member";
                        return RedirectToAction("Index", "Home");
                    }

                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Product) exceptionEntry.Entity;
                    var databaseEntry = await exceptionEntry.GetDatabaseValuesAsync();
                    var databaseValues = (Product) databaseEntry.ToObject();

                    if (databaseValues.Name != clientValues.Name)
                        ModelState.AddModelError("Product.Name", $"Current value: {databaseValues.Name}");
                    if (databaseValues.Description != clientValues.Description)
                        ModelState.AddModelError("Product.Description", $"Current value: {databaseValues.Description}");
                    if (databaseValues.Price != clientValues.Price)
                        ModelState.AddModelError("Product.Price", $"Current value: {databaseValues.Price}");

                    if (model.IdsOfSelectedCategories != null)
                    {
                        var allCategories = await _navigationService.GetAllCategories();
                        var allProductCategories = await _navigationService.GetAllProductCategories();
                        var dbProductCategoryNames = await Task.Run(() =>
                            allProductCategories.Where(x => x.Product == model.Product).Select(x => x.Category.Name)
                                .ToArray());
                        var clientProductCategoryNames = await Task.Run(() =>
                            allCategories.Where(x => model.IdsOfSelectedCategories.Contains(x.Id)).Select(x => x.Name)
                                .ToArray());
                        if (!dbProductCategoryNames.ToHashSet().SetEquals(clientProductCategoryNames.ToHashSet()))
                        {
                            var categoryStrings = string.Join(", ", dbProductCategoryNames);
                            ModelState.AddModelError("IdsOfSelectedCategories", $"Current value: {categoryStrings}");
                        }
                    }


                    ModelState.AddModelError(string.Empty,
                        "The product's values were updated while you were editing them. Review the changes (if any) " +
                        "and if you still wish to submit them, click the 'Save' button again. Otherwise, refresh the page to get the updated data.");

                    await FillUpProductEditData(model, model.Product);

                    model.Product.RowVersion = databaseValues.RowVersion;
                    ModelState.Remove("Product.RowVersion");

                    return View(model);
                }

                TempData["SuccessMsg"] = "Product updated successfully.";
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMsg"] = "Product has been deleted by another staff member";
                return RedirectToAction("Index", "Home");
            }

            var product = await _productService.FindProductByIdAsync((int) id);

            if (product == null)
            {
                TempData["ErrorMsg"] = "Product has been deleted by another staff member";
                return RedirectToAction("Index", "Home");
            }

            return View(product);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productService.FindProductByIdAsync(id);

            if (product != null)
            {
                var images = await _productService.GetAllProductImages(product.Id);

                //Remove images
                if (images != null && images.Count > 0)
                    foreach (var image in images)
                    {
                        await _appEnvironment.DeleteImageAsync(image.ImageUrl, "products");
                        await _productService.DeleteProductImage(image);
                    }

                await _productService.DeleteProduct(product.Id);
                return RedirectToAction(nameof(Index), new {showAlert = true});
            }
            TempData["ErrorMsg"] = "Product has been deleted by another staff member";
            return RedirectToAction("Index", "Home");
        }

        //Product properties management below
        //Page with all product properties with "delete" button
        public async Task<IActionResult> ManageProperties(int id, bool showAlert = false)
        {
            var temp = await _productService.FindProductByIdAsync(id);
            //Using ViewData to retrieve values in view
            ViewData["product_name"] = temp.Name;
            ViewData["product_id"] = id;
            //ViewData["show_alert"] = showAlert;
            return View(await _productService.GetAllPropertiesByProductIdAsync(id));
        }

        //Page with add property form
        public async Task<IActionResult> AddProperty(int productId) //Add Property view
        {
            var temp = await _productService.FindProductByIdAsync(productId);
            ViewData["product_name"] = temp.Name;
            ViewData["product_id"] = temp.Id;
            return View();
        }

        //Add property action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProperty(int productId, [Bind("Id,Name,Description,ProductId")]
            ProductProperty productProperty)
        {
            if (ModelState.IsValid)
            {
                await _productService.CreateProductProperty(productProperty);
                return RedirectToAction(nameof(ManageProperties), new {id = productId});
            }

            //If admin did not fill every field then refresh page
            var temp = await _productService.FindProductByIdAsync(productId);
            ViewData["product_name"] = temp.Name;
            ViewData["product_id"] = temp.Id;
            return View();
        }

        //Product property delete button action
        public async Task<IActionResult> RemoveProductProperty(int id)
        {
            var property = await _productService.FindProductPropertyByIdAsync(id);
            var productId = property.ProductId;

            try
            {
                await _productService.DeleteProductProperty(property.Id);

                return RedirectToAction(nameof(ManageProperties), new {id = productId});
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<ProductCategoryViewModel> FillUpProductEditData(ProductCategoryViewModel model,
            Product product)
        {
            var categories = await _navigationService.GetAllCategories();
            var productCategories = await _navigationService.GetAllProductCategories();
            IList<int> idsOfSelectedCategories = productCategories.Where(pc => pc.ProductId == product.Id)
                .Select(pc => pc.CategoryId).ToList();


            var primaryImages = await _productService.GetProductImages(product.Id);
            var otherImages = await _productService.GetProductImages(product.Id, false);

            model.Product = product;
            model.CategoryMultiSelectList = new MultiSelectList(categories, "Id", "Name", idsOfSelectedCategories);
            model.ImagesToRemoveSelectList = new MultiSelectList(otherImages, "Id", "ImageUrl");

            ViewBag.PrimaryImage = primaryImages.Count > 0 ? primaryImages[0].ImageUrl : "product-image-placeholder.jpg";
            ViewBag.OtherImages = otherImages;
            ViewBag.UploadMaxMbSize = _uploadMaxByteSize / 1048576;

            return model;
        }

        [HttpGet]
        public async Task<IActionResult> Discount(string page, int productId)
        {
            ViewBag.Product = await _productService.FindProductByIdAsync(productId);
            ViewData["page"] = page;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Discount(string page, ProductDiscount productDiscount)
        {
            ViewBag.Product = await _productService.FindProductByIdAsync(productDiscount.ProductId);
            try
            {
                if (ModelState.IsValid
                ) // && await _context.ProductDiscount.FirstOrDefaultAsync(pd => pd.ProductId == productDiscount.ProductId) == null) //if there is no discount for this product
                {
                    await _productService.CreateDiscount(productDiscount);
                    if (page == "Index")
                        return RedirectToAction("Index", "Home");
                    return RedirectToAction("ProductPage", "Home", new {id = productDiscount.ProductId});
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "This product already has a discount.");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RemoveDiscount(string page, int productId)
        {
            var productDiscount = await _productService.GetDiscountByProductId(productId);

            await _productService.DeleteDiscount(productDiscount);
            if (page == "Index")
                return RedirectToAction("Index", "Home");
            return RedirectToAction("ProductPage", "Home", new {id = productId});
        }
    }
}