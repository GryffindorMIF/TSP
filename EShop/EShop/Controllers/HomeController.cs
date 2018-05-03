﻿using System;
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
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using EShop.Util;

namespace EShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INavigationService _navigationService;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly int uploadMaxByteSize;
        private readonly int productsPerPage;

        private const int startingPageNumber = 0;

        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, INavigationService navigationService, IConfiguration configuration, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _navigationService = navigationService;
            _appEnvironment = appEnvironment;

            if (!int.TryParse(configuration["ProductsConfig:ProductsPerPage"], out productsPerPage))
            {
                throw new InvalidOperationException("Invalid ProductsConfig:ProductsPerPage in appsettings.json. Not an int value.");
            }
            if (!int.TryParse(configuration["FileManagerConfig:UploadMaxByteSize"], out uploadMaxByteSize))
            {
                throw new InvalidOperationException("Invalid FileManagerConfig:UploadMaxByteSize in appsettings.json. Not an int value.");
            }
        }

        // GET, POST
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? categoryId = null, bool backToParentCategory = false, string absoluteNavigationPath = null, string navigateToCategoryNamed = null)
        {
            ViewBag.CurrentPageNumber = startingPageNumber;
            ViewBag.PreviousPageNumber = null;
            ViewBag.NextPageNumber = 1;

            Category currentCategory = null;
            ICollection<Product> productsToView = null;// products to return
            ICollection<Category> selectableCategories = null;// navigation menu

            if (categoryId == null)
            {
                if (navigateToCategoryNamed == null)// (GET)
                {
                    // build menu
                    selectableCategories = await _navigationService.GetTopLevelCategoriesAsync();

                    //ViewBag.ParentCategoryId = null;
                    //ViewBag.AbsoluteNavigationPath = null;
                    ViewBag.CurrentCategoryId = categoryId;

                    productsToView = await _navigationService.GetProductsInCategoryByPageAsync(null, startingPageNumber, productsPerPage);
                }
                else// (POST) specific path segment selected ([segment]/.../...)
                {
                    // get that category by name
                    currentCategory = await (from c in _context.Category
                                             where c.Name == navigateToCategoryNamed
                                             select c).FirstAsync();
                    // build menu
                    selectableCategories = await _navigationService.GetChildCategoriesAsync(currentCategory);

                    // products to return
                    productsToView = await _navigationService.GetProductsInCategoryByPageAsync(currentCategory, startingPageNumber, productsPerPage);

                    // get parent category by absoluteNavigationPath
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
                                                     select c).FirstAsync();// category name is unique

                    ViewBag.ParentCategoryId = parentCategory.Id;
                    ViewBag.AbsoluteNavigationPath = newAbsoluteNavigationPath;
                    ViewBag.CurrentCategoryName = parentCategory.Name;
                    ViewBag.CurrentCategoryId = currentCategory.Id;
                }
            }
            else// (POST) backward and forward navigation
            {
                currentCategory = await _context.Category.FindAsync(categoryId);

                if (backToParentCategory)// Backward navigation
                {
                    absoluteNavigationPath = await _navigationService.RemoveLastUriSegmentAsync(absoluteNavigationPath);
                    ViewBag.AbsoluteNavigationPath = absoluteNavigationPath;

                    selectableCategories = null;// navigation menu
                    Category parentCategory = null;

                    // categories may have multiple parent-categories
                    ICollection<Category> parentCategories = await _navigationService.GetParentCategoriesAsync(currentCategory);

                    if (!parentCategories.Any())// already a top-level category
                    {
                        selectableCategories = await _navigationService.GetTopLevelCategoriesAsync();

                        ViewBag.ParentCategoryId = null;
                        ViewBag.CurrentCategoryId = currentCategory.Id;
                        ViewBag.AbsoluteNavigationPath = null;
                        ViewBag.CurrentCategoryName = null;

                        productsToView = await _navigationService.GetProductsInCategoryByPageAsync(null, startingPageNumber, productsPerPage);

                        // for later page count calculation
                        currentCategory = null;
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
                        // build menu
                        selectableCategories = await _navigationService.GetChildCategoriesAsync(parentCategory);

                        ViewBag.CurrentCategoryName = parentCategory.Name;
                        ViewBag.ParentCategoryId = parentCategory.Id;
                        ViewBag.CurrentCategoryId = parentCategory.Id;

                        // products to return
                        productsToView = await _navigationService.GetProductsInCategoryByPageAsync(parentCategory, startingPageNumber, productsPerPage);

                        // for later page count calculation
                        currentCategory = parentCategory;
                    }
                }
                else // Forward navigation
                {
                    // Add new segment to absoluteNavigationPath
                    absoluteNavigationPath += ("/" + currentCategory.Name);
                    ViewBag.AbsoluteNavigationPath = absoluteNavigationPath;

                    // build menu
                    selectableCategories = selectableCategories = await _navigationService.GetChildCategoriesAsync(currentCategory);

                    ViewBag.CurrentCategoryName = currentCategory.Name;
                    ViewBag.ParentCategoryId = categoryId;
                    ViewBag.CurrentCategoryId = currentCategory.Id;

                    // products to return
                    if (categoryId == null) productsToView = await _navigationService.GetProductsInCategoryByPageAsync(null, startingPageNumber, productsPerPage);
                    else productsToView = await _navigationService.GetProductsInCategoryByPageAsync(currentCategory, startingPageNumber, productsPerPage);
                }
            }

            int pageCount = await _navigationService.GetProductsInCategoryPageCount(currentCategory, productsPerPage);
            ViewBag.PageCount = pageCount;

            if (startingPageNumber + 1 < pageCount) ViewBag.NextPageNumber = startingPageNumber + 1;
            else ViewBag.NextPageNumber = null;

            ViewBag.TopLevelCategories = selectableCategories;

            String[] allPrimaryImageLinks = new String[productsToView.Count];

            await Task.Run(() =>
            {
                var listProducts = productsToView.ToList();
                for (int i = 0; i < listProducts.Count; i++)
                {
                    List<ProductImage> primaryImage = (from pi in _context.ProductImage
                                                       where pi.IsPrimary
                                                       where pi.Product == listProducts[i]
                                                       select pi).ToList();
                    if (primaryImage.Count > 0)
                    {
                        allPrimaryImageLinks[i] = primaryImage[0].ImageUrl;
                    }
                    else
                    {
                        allPrimaryImageLinks[i] = "product-image-placeholder.jpg";
                    }
                }
            });
            ViewBag.AllPrimaryImageLinks = allPrimaryImageLinks;
            // -----------------------------------------
            ICollection<ProductAd> productAds = await _context.ProductAd.ToListAsync();
            ViewBag.ProductAds = productAds;
            // -----------------------------------------
            ICollection<ProductDiscount> discountList = await _context.ProductDiscount.ToListAsync();

            IList<bool> hasDiscountList = new List<bool>();
            IList<Decimal?> discountPriceList = new List<Decimal?>();
            IList<DateTime?> dicountEndDateList = new List<DateTime?>();

            foreach (var product in productsToView.Select((value, index) => new { Value = value, Index = index }))
            {
                hasDiscountList.Add(false);
                discountPriceList.Add(null);
                dicountEndDateList.Add(null);

                foreach (var discount in discountList)
                {
                    if (product.Value.Id == discount.ProductId)
                    {
                        if (discount.Ends > DateTime.Now)
                        {
                            hasDiscountList[product.Index] = true;
                            discountPriceList[product.Index] = discount.DiscountPrice;
                            dicountEndDateList[product.Index] = discount.Ends;
                        }
                        else
                        {
                            _context.Remove(discount);
                            await _context.SaveChangesAsync();
                        }
                        break;
                    }
                }
            }
            ViewBag.HasDiscountList = hasDiscountList;
            ViewBag.DiscountPriceList = discountPriceList;
            ViewBag.DiscountEndDateList = dicountEndDateList;

            return View(productsToView);
        }

        [AllowAnonymous]
        public async Task<IActionResult> LoadPage(int pageCount, int? categoryId = null, int? parentCategoryId = null, ICollection<Category> topLevelCategories = null, string absoluteNavigationPath = null, int pageNumber = startingPageNumber)
        {
            ViewBag.ParentCategoryId = parentCategoryId;
            ViewBag.AbsoluteNavigationPath = absoluteNavigationPath;
            ViewBag.CurrentCategoryId = categoryId;
            ViewBag.CurrentPageNumber = pageNumber;
            ViewBag.PageCount = pageCount;

            if (pageNumber + 1 < pageCount) ViewBag.NextPageNumber = pageNumber + 1;
            else ViewBag.NextPageNumber = null;
            if (pageNumber > 0) ViewBag.PreviousPageNumber = pageNumber - 1;
            else ViewBag.PreviousPageNumber = null;

            Category category = null;
            if (parentCategoryId != null) category = await _context.Category.FindAsync(parentCategoryId);

            if (category == null) 
            {
                ViewBag.TopLevelCategories = await _navigationService.GetTopLevelCategoriesAsync();
                ViewBag.CurrentCategoryName = null;
                ViewBag.AbsoluteNavigationPath = null;
            }
            else
            {
                ViewBag.TopLevelCategories = await _navigationService.GetChildCategoriesAsync(category);
                ViewBag.CurrentCategoryName = category.Name;
            }

            ICollection<Product> products = await _navigationService.GetProductsInCategoryByPageAsync(category, pageNumber, productsPerPage);
            var listProducts = products.ToList();

            String[] allPrimaryImageLinks = new String[products.Count];

            await Task.Run(() =>
            {           
                for (int i = 0; i < listProducts.Count; i++)
                {
                    List<ProductImage> primaryImage = (from pi in _context.ProductImage
                                                       where pi.IsPrimary
                                                       where pi.Product == listProducts[i]
                                                       select pi).ToList();
                    if (primaryImage.Count > 0)
                    {
                        allPrimaryImageLinks[i] = primaryImage[0].ImageUrl;
                    }
                    else
                    {
                        allPrimaryImageLinks[i] = "product-image-placeholder.jpg";
                    }
                }
            });

            ViewBag.AllPrimaryImageLinks = allPrimaryImageLinks;
            // -----------------------------------------
            ICollection<ProductAd> productAds = await _context.ProductAd.ToListAsync();
            ViewBag.ProductAds = productAds;
            // -----------------------------------------
            ICollection<ProductDiscount> discountList = await _context.ProductDiscount.ToListAsync();

            IList<bool> hasDiscountList = new List<bool>();
            IList<Decimal?> discountPriceList = new List<Decimal?>();
            IList<DateTime?> dicountEndDateList = new List<DateTime?>();

            foreach (var product in listProducts.Select((value, index) => new { Value = value, Index = index }))
            {
                hasDiscountList.Add(false);
                discountPriceList.Add(null);
                dicountEndDateList.Add(null);

                foreach (var discount in discountList)
                {
                    if (product.Value.Id == discount.ProductId)
                    {
                        if (discount.Ends > DateTime.Now)
                        {
                            hasDiscountList[product.Index] = true;
                            discountPriceList[product.Index] = discount.DiscountPrice;
                            dicountEndDateList[product.Index] = discount.Ends;
                        }
                        else
                        {
                            _context.Remove(discount);
                            await _context.SaveChangesAsync();
                        }
                        break;
                    }
                }
            }
            ViewBag.HasDiscountList = hasDiscountList;
            ViewBag.DiscountPriceList = discountPriceList;
            ViewBag.DiscountEndDateList = dicountEndDateList;

            return View("Index", products);
        }

        //Denis added product page, not finished yet
        [AllowAnonymous]
        public async Task<IActionResult> ProductPage(int id)
        {
            Product temp = _context.Product.Single(p => p.Id == id);
            ViewBag.Product = temp;
            await Task.Run(() => //Loading primary image and in future should start loading all images
            {
                try //If product has an image
                {
                    ProductImage primaryImage = _context.ProductImage.First(pi => pi.IsPrimary && pi.Product == temp);
                    ViewData["primary_image"] = primaryImage.ImageUrl;
                    //ViewBag.PrimaryImage = primaryImage.ImageUrl;
                    List<ProductImage> secondaryImages = _context.ProductImage.Where(pi => !pi.IsPrimary && pi.Product.Id == temp.Id).ToList();
                    ViewBag.SecondaryImages = secondaryImages;
                }
                catch (Exception) //Could appear if product doesn't have any photos
                {
                    ViewData["primary_image"] = "product-image-placeholder.jpg"; //Then just set placeholder
                    ViewBag.SecondaryImages = new List<ProductImage>();
                }
            });

            ProductDiscount discount = await (from pd in _context.ProductDiscount
                            where pd.ProductId == id
                            select pd).FirstOrDefaultAsync();

            if (discount != null)
            {
                if (discount.Ends > DateTime.Now)
                {
                    ViewBag.Discount = discount;
                }
                else
                {
                    _context.Remove(discount);
                    await _context.SaveChangesAsync();
                }
            }

            return View(await _context.ProductDetails.Where(p => p.ProductId == id).ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditMainCarousel()
        {
            ProductAdViewModel productAdViewModel = new ProductAdViewModel();

            List<Product> products = null;
            List<ProductAd> productAds = null;

            await Task.Run(() =>
            {
                products = (from p in _context.Product
                              select p).ToList();

                productAds = (from pa in _context.ProductAd
                               select pa).ToList();
            

                productAdViewModel.ProductSelectList = new SelectList(products, "Id", "Name");
                productAdViewModel.AdsToRemoveSelectList = new MultiSelectList(productAds, "Id", "Product.Name");

                ViewBag.ProductAds = productAds;
                ViewBag.UploadMaxMbSize = uploadMaxByteSize / 1048576;
            });

            return View("EditMainCarousel", productAdViewModel);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAd(ProductAdViewModel productAdViewModel)
        {
            IList<ProductAd> possibleAdImages = null;

            var task = Task.Run(() =>
            {
                if (productAdViewModel.ProductAdImage != null)
                {
                    possibleAdImages = (from pai in _context.ProductAd
                                             where pai.Product.Id == productAdViewModel.SelectedProductId
                                             select pai).ToList();
                }
            });
            task.Wait();

            var img = productAdViewModel.ProductAdImage;

            if (productAdViewModel.ProductAdImage != null)
            {
                var adImagePath = await _appEnvironment.UploadImageAsync(productAdViewModel.ProductAdImage, "main carousel", uploadMaxByteSize);
                if (adImagePath != null)
                {
                    ProductAd productAd = new ProductAd
                    {
                        AdImageUrl = adImagePath,
                        Product = await _context.Product.FindAsync(productAdViewModel.SelectedProductId)
                    };
                    if (possibleAdImages != null && possibleAdImages.Count > 0)
                    {
                        await _appEnvironment.DeleteImageAsync(possibleAdImages[0].AdImageUrl, "main carousel");
                        _context.Remove(possibleAdImages[0]);
                    }
                    _context.Add(productAd);
                    await _context.SaveChangesAsync();
                }
            }
            return await EditMainCarousel();
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAds(ProductAdViewModel productAdViewModel)
        {
            foreach(var adId in productAdViewModel.IdsOfSelectedAdsToRemove)
            {
               var adToRemove = await _context.ProductAd.FindAsync(adId);
                _context.Remove(adToRemove);
                await _appEnvironment.DeleteImageAsync(adToRemove.AdImageUrl, "main carousel");
            }
            await _context.SaveChangesAsync();

            return await EditMainCarousel();
        }
    }
}
