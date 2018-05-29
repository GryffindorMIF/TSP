using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EShop.Business.Interfaces;
using EShop.Models.EFModels.Attribute;
using EShop.Models.EFModels.Category;
using EShop.Models.EFModels.Product;
using EShop.Models.ViewModels;
using EShop.Models.ViewModels.Home;
using EShop.Models.ViewModels.Product;
using EShop.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Attribute = EShop.Models.EFModels.Attribute.Attribute;

namespace EShop.Controllers
{
    public class HomeController : Controller
    {
        private const int StartingPageNumber = 0;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly IAttributeService _attributeService;
        private readonly INavigationService _navigationService;
        private readonly IProductService _productService;
        private readonly int _productsPerPage;
        private readonly int _uploadMaxByteSize;

        public HomeController(INavigationService navigationService,
            IConfiguration configuration, IHostingEnvironment appEnvironment, IProductService productService,
            IAttributeService attributeService)
        {
            _navigationService = navigationService;
            _appEnvironment = appEnvironment;
            _productService = productService;
            _attributeService = attributeService;

            if (!int.TryParse(configuration["PaginationConfig:ProductsPerPage"], out _productsPerPage))
                throw new InvalidOperationException(
                    "Invalid PaginationConfig:ProductsPerPage in appsettings.json. Not an int value.");
            if (!int.TryParse(configuration["FileManagerConfig:UploadMaxByteSize"], out _uploadMaxByteSize))
                throw new InvalidOperationException(
                    "Invalid FileManagerConfig:UploadMaxByteSize in appsettings.json. Not an int value.");
        }

        // GET, POST
        [AllowAnonymous]
        public async Task<IActionResult> Index(bool isSearch = false, string searchText = "", int? categoryId = null,
            bool backToParentCategory = false, string absoluteNavigationPath = null,
            string navigateToCategoryNamed = null)
        {
            ViewBag.CurrentPageNumber = StartingPageNumber;
            ViewBag.PreviousPageNumber = null;
            ViewBag.NextPageNumber = 1;
            ViewBag.SearchText = searchText; //Used for search pagination :(
            ViewBag.IsSearch = isSearch; //Used for search pagination :(

            Category currentCategory = null;
            ICollection<Product> filteredProducts = null; //Used for search

            if (isSearch)
                filteredProducts = await _productService.SearchForProducts(searchText);
            ICollection<Category> selectableCategories; // navigation menu

            ICollection<Product> productsToView; // products to return in current page
            if (categoryId == null)
            {
                if (navigateToCategoryNamed == null) // (GET)
                {
                    // build menu
                    selectableCategories = await _navigationService.GetTopLevelCategoriesAsync();

                    //ViewBag.ParentCategoryId = null;
                    //ViewBag.AbsoluteNavigationPath = null;
                    ViewBag.CurrentCategoryId = null;

                    if (!isSearch)
                        productsToView =
                            await _navigationService.GetProductsInCategoryByPageAsync(null, StartingPageNumber,
                                _productsPerPage);
                    else
                        productsToView = filteredProducts.Skip(StartingPageNumber * _productsPerPage)
                            .Take(_productsPerPage).ToList();
                }
                else // (POST) specific path segment selected ([segment]/.../...)
                {
                    // get that category by name
                    currentCategory = await _navigationService.GetCategoryByName(navigateToCategoryNamed);

                    // build menu
                    selectableCategories = await _navigationService.GetChildCategoriesAsync(currentCategory);

                    // products to return
                    productsToView =
                        await _navigationService.GetProductsInCategoryByPageAsync(currentCategory, StartingPageNumber,
                            _productsPerPage);

                    // get parent category by absoluteNavigationPath
                    if (absoluteNavigationPath != null)
                    {
                        var pathSegments = absoluteNavigationPath.Split("/");
                        pathSegments =
                            pathSegments.Skip(1).ToArray(); // remove empty segment ([empty segment]/[some segment]/...)

                        var parentCategoryIndexInPath = Array.IndexOf(pathSegments, navigateToCategoryNamed);
                        string newAbsoluteNavigationPath = null;
                        string parentCategoryName = null;

                        await Task.Run(() =>
                        {
                            for (var i = 0; i < pathSegments.Count(); i++)
                            {
                                if (i == parentCategoryIndexInPath) parentCategoryName = pathSegments[i];
                                if (i <= parentCategoryIndexInPath) newAbsoluteNavigationPath += "/" + pathSegments[i];
                            }
                        });

                        var parentCategory = await _navigationService.GetCategoryByName(parentCategoryName);

                        ViewBag.ParentCategoryId = parentCategory.Id;
                        ViewBag.AbsoluteNavigationPath = newAbsoluteNavigationPath;
                        ViewBag.CurrentCategoryName = parentCategory.Name;
                    }

                    ViewBag.CurrentCategoryId = currentCategory.Id;
                }
            }
            else // forward navigation
            {
                currentCategory = await _navigationService.GetCategoryById(categoryId);
                /*
                if (backToParentCategory) // Backward navigation
                {
                    absoluteNavigationPath = await _navigationService.RemoveLastUriSegmentAsync(absoluteNavigationPath);
                    ViewBag.AbsoluteNavigationPath = absoluteNavigationPath;

                    Category parentCategory = null;

                    // categories may have multiple parent-categories
                    var parentCategories = await _navigationService.GetParentCategoriesAsync(currentCategory);

                    if (!parentCategories.Any()) // already a top-level category
                    {
                        selectableCategories = await _navigationService.GetTopLevelCategoriesAsync();

                        ViewBag.ParentCategoryId = null;
                        //ViewBag.CurrentCategoryId = currentCategory.Id;
                        ViewBag.CurrentCategoryId = null;
                        ViewBag.AbsoluteNavigationPath = null;
                        ViewBag.CurrentCategoryName = null;

                        productsToView =
                            await _navigationService.GetProductsInCategoryByPageAsync(null, StartingPageNumber,
                                _productsPerPage);

                        // for later page count calculation
                        currentCategory = null;
                    }
                    else // not a top-level category
                    {
                        foreach (var pCategory in parentCategories)
                            // search for parent-category based on absoluteNavigationPath last segment
                            if (pCategory.Name.Equals(absoluteNavigationPath.Split('/').Last()))
                            {
                                parentCategory = pCategory;
                                break;
                            }

                        // build menu
                        selectableCategories = await _navigationService.GetChildCategoriesAsync(parentCategory);

                        if (parentCategory != null)
                        {
                            ViewBag.CurrentCategoryName = parentCategory.Name;
                            ViewBag.ParentCategoryId = parentCategory.Id;
                            ViewBag.CurrentCategoryId = parentCategory.Id;

                            productsToView =
                                await _navigationService.GetProductsInCategoryByPageAsync(parentCategory,
                                    StartingPageNumber, _productsPerPage);

                            // for later page count calculation
                            currentCategory = parentCategory;
                        }
                        else
                        {
                            // products to return
                            productsToView =
                                await _navigationService.GetProductsInCategoryByPageAsync(null,
                                    StartingPageNumber, _productsPerPage);

                            currentCategory = null;
                        }
                    }
                }
                else // Forward navigation
                {
                */
                    var parentCategoryName = "";

                    if (absoluteNavigationPath != null)
                    {
                        parentCategoryName = absoluteNavigationPath.Split('/').Last().Trim();
                        if (parentCategoryName.Equals("")) parentCategoryName = null;
                    }
                    else parentCategoryName = null;

                    var parentCategory = await _navigationService.GetCategoryByName(parentCategoryName);

                    // Add new segment to absoluteNavigationPath
                    absoluteNavigationPath += "/" + currentCategory.Name;
                    ViewBag.AbsoluteNavigationPath = absoluteNavigationPath;

                    // build menu
                    selectableCategories = await _navigationService.GetChildCategoriesAsync(currentCategory);

                    ViewBag.CurrentCategoryName = currentCategory.Name;
                    ViewBag.ParentCategoryId = parentCategory?.Id;
                    ViewBag.CurrentCategoryId = currentCategory.Id;

                    // products to return
                    productsToView =
                        await _navigationService.GetProductsInCategoryByPageAsync(currentCategory,
                            StartingPageNumber, _productsPerPage);
                //}
            }

            int pageCount;
            if (!isSearch)
            {
                pageCount = await _navigationService.GetProductsInCategoryPageCount(currentCategory, _productsPerPage);
            }
            else //If search
            {
                pageCount = filteredProducts.Count / _productsPerPage;
                if (filteredProducts.Count % _productsPerPage != 0) pageCount++;
            }

            ViewBag.PageCount = pageCount;

            if (StartingPageNumber + 1 < pageCount) ViewBag.NextPageNumber = StartingPageNumber + 1;
            else ViewBag.NextPageNumber = null;

            ViewBag.TopLevelCategories = selectableCategories;

            var allPrimaryImageLinks = await _productService.GetProductsImagesLinks(productsToView);

            ViewBag.AllPrimaryImageLinks = allPrimaryImageLinks;
            // -----------------------------------------
            ICollection<ProductAd> productAds = await _productService.GetProductAds();
            ViewBag.ProductAds = productAds;

            //------- discountai ------------//
            var dlvm = await GetDiscountList(productsToView);
            ViewBag.HasDiscountList = dlvm.HasDiscountList;
            ViewBag.DiscountPriceList = dlvm.DiscountPriceList;
            ViewBag.DiscountEndDateList = dlvm.DiscountEndDateList;

            //------- atributai ------------//
            var alvm = await GetAttributeListInCategory(currentCategory);
            ViewBag.AttributeValues = alvm.AttributeValues;
            ViewBag.Attributes = alvm.Attributes;

            //category management
            CategoryListViewModel clvm = new CategoryListViewModel()
            {
                Categories = new SelectList(await _navigationService.GetAllCategories(), "Id", "Name")
            };
            ViewBag.AllCategories = clvm;
            return View(productsToView);
        }

        [AllowAnonymous]
        public async Task<IActionResult> LoadPage(string attributeName, bool isSearch,
            string searchText, /* redundant --> */int pageCount, int? categoryId = null, int? parentCategoryId = null,
            ICollection<Category> topLevelCategories = null, string absoluteNavigationPath = null,
            int pageNumber = StartingPageNumber)
        {
            ViewBag.ParentCategoryId = parentCategoryId;
            ViewBag.AbsoluteNavigationPath = absoluteNavigationPath;
            ViewBag.CurrentCategoryId = categoryId;
            ViewBag.CurrentPageNumber = pageNumber;
            ViewBag.PageCount = pageCount;
            ViewBag.IsSearch = isSearch; //Used for search pagination
            ViewBag.SearchText = searchText; //Used for search pagination

            if (pageNumber + 1 < pageCount) ViewBag.NextPageNumber = pageNumber + 1;
            else ViewBag.NextPageNumber = null;
            if (pageNumber > 0) ViewBag.PreviousPageNumber = pageNumber - 1;
            else ViewBag.PreviousPageNumber = null;

            Category category = null;
            if (parentCategoryId != null) category = await _navigationService.GetCategoryById(parentCategoryId);

            ICollection<Product> products;
            if (!isSearch)
            {
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

                products = await _navigationService.GetProductsInCategoryByPageAsync(category, pageNumber,
                    _productsPerPage);
            }
            else //If it's search state
            {
                //To prevent null reference
                ViewBag.TopLevelCategories = await _navigationService.GetTopLevelCategoriesAsync();
                ViewBag.CurrentCategoryName = null;
                ViewBag.AbsoluteNavigationPath = null;

                products = await _productService.GetAllProducts();
                products = products.Where(p => p.Name.Contains(searchText)).ToList();
                products = products.Skip(pageNumber * _productsPerPage).Take(_productsPerPage).ToList();
            }

            var listProducts = products.ToList();

            var allPrimaryImageLinks =
                await _productService.GetProductsImagesLinks(products); //Retrieve all image links

            ViewBag.AllPrimaryImageLinks = allPrimaryImageLinks;
            // -----------------------------------------
            ICollection<ProductAd> productAds = await _productService.GetProductAds();
            ViewBag.ProductAds = productAds;

            //------- discountai ------------//
            var dlvm = await GetDiscountList(listProducts);
            ViewBag.HasDiscountList = dlvm.HasDiscountList;
            ViewBag.DiscountPriceList = dlvm.DiscountPriceList;
            ViewBag.DiscountEndDateList = dlvm.DiscountEndDateList;

            //------- atributai ------------//
            var alvm = await GetAttributeListInCategory(category);
            ViewBag.AttributeValues = alvm.AttributeValues;
            ViewBag.Attributes = alvm.Attributes;
            ViewBag.SelectedAttributeValueName = attributeName;

            //category management
            CategoryListViewModel clvm = new CategoryListViewModel()
            {
                Categories = new SelectList(await _navigationService.GetAllCategories(), "Id", "Name")
            };
            ViewBag.AllCategories = clvm;
            return View("Index", products);
        }

        // ProductPage index
        [AllowAnonymous]
        public async Task<IActionResult> ProductPage(int id)
        {
            //Product temp = await _context.Product.FirstOrDefaultAsync(p => p.Id == id);
            var temp = await _productService.FindProductByIdAsync(id);
            //List<Product> products = new List<Product>();
            //products.Add(temp);
            ViewBag.Product = temp;

            var primaryImage = await _productService.GetProductImages(temp.Id);

            ViewData["primary_image"] =
                primaryImage.Count == 0 ? "product-image-placeholder.jpg" : primaryImage[0].ImageUrl;

            var secondaryImages = await _productService.GetProductImages(temp.Id, false);
            //String[] secondaryImages = await _productService.GetProductsImagesLinks(products, false);
            ViewBag.SecondaryImages = secondaryImages;

            var discount = await _productService.GetDiscountByProductId(id);
            if (discount != null)
            {
                if (discount.Ends > DateTime.Now)
                    ViewBag.Discount = discount;
                else
                    await _productService.DeleteDiscount(discount);
            }

            ICollection<AttributeValue> attributes = await _attributeService.GetProductAttributeValues(id);

            ICollection<Attribute> attributeCategories = new List<Attribute>();
            foreach (var attr in attributes)
            {
                var attrCategory = _attributeService.FindAttributeById(attr.AttributeId);
                if (!attributeCategories.Contains(attrCategory)) attributeCategories.Add(attrCategory);
            }

            ViewBag.Attributes = attributes;
            ViewBag.AttributeCategories = attributeCategories;

            return View(await _productService.GetAllPropertiesByProductIdAsync(id));
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> EditMainCarousel()
        {
            var productAdViewModel = new ProductAdViewModel();

            var products = await _productService.GetAllProducts();

            ICollection<ProductAd> productAds = await _productService.GetProductAds();


            productAdViewModel.ProductSelectList = new SelectList(products, "Id", "Name");
            productAdViewModel.AdsToRemoveSelectList = new MultiSelectList(productAds, "Id", "Product.Name");

            ViewBag.ProductAds = productAds;
            ViewBag.UploadMaxMbSize = _uploadMaxByteSize / 1048576;

            return View("EditMainCarousel", productAdViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> CreateAd(ProductAdViewModel productAdViewModel)
        {
            var possibleAdImages = await _productService.ListPossibleAdImages(productAdViewModel.SelectedProductId);

            if (productAdViewModel.ProductAdImage != null)
            {
                var adImagePath = await _appEnvironment.UploadImageAsync(productAdViewModel.ProductAdImage,
                    "main carousel", _uploadMaxByteSize);
                if (adImagePath != null)
                {
                    var productAd = new ProductAd
                    {
                        AdImageUrl = adImagePath,
                        Product = await _productService.FindProductByIdAsync(productAdViewModel.SelectedProductId)
                    };
                    if (possibleAdImages != null && possibleAdImages.Count > 0)
                    {
                        await _appEnvironment.DeleteImageAsync(possibleAdImages[0].AdImageUrl, "main carousel");
                        await _productService.DeleteProductAd(possibleAdImages[0]);
                    }

                    await _productService.CreateProductAd(productAd);
                }
            }

            return await EditMainCarousel();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> DeleteAds(ProductAdViewModel productAdViewModel)
        {
            foreach (var adId in productAdViewModel.IdsOfSelectedAdsToRemove)
                try
                {
                    var adToRemove = await _productService.GetProductAdById(adId);
                    await _appEnvironment.DeleteImageAsync(adToRemove.AdImageUrl, "main carousel");
                    await _productService.DeleteProductAd(adToRemove);
                }
                catch (Exception) // kazkas jau anksciau removino ad'a
                {
                    // ignored
                }

            return await EditMainCarousel();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> FilterByAttributes(bool isSearch, string searchText, string attributeName,
            int pageCount, int? categoryId = null, int? parentCategoryId = null,
            ICollection<Category> topLevelCategories = null, string absoluteNavigationPath = null,
            int pageNumber = StartingPageNumber)
        {
            if (pageCount <= 0) throw new ArgumentOutOfRangeException(nameof(pageCount));
            ViewBag.ParentCategoryId = parentCategoryId;
            ViewBag.AbsoluteNavigationPath = absoluteNavigationPath;
            ViewBag.CurrentCategoryId = categoryId;
            ViewBag.CurrentPageNumber = pageNumber;

            ViewBag.IsSearch = isSearch; //Used for search pagination
            ViewBag.SearchText = searchText; //Used for search pagination

            Category category = null;
            //if (parentCategoryId != null) category = await _navigationService.GetCategoryById(parentCategoryId);
            category = await _navigationService.GetCategoryById(categoryId);

            pageCount = await _navigationService.GetProductsInCategoryPageCount(category, _productsPerPage,
                attributeName);
            ViewBag.PageCount = pageCount;

            if (pageNumber + 1 < pageCount) ViewBag.NextPageNumber = pageNumber + 1;
            else ViewBag.NextPageNumber = null;
            if (pageNumber > 0) ViewBag.PreviousPageNumber = pageNumber - 1;
            else ViewBag.PreviousPageNumber = null;

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

            var products =
                await _navigationService.GetProductsInCategoryByPageAsync(category, pageNumber, _productsPerPage,
                    attributeName);
            var listProducts = products.ToList();

            //--------- images --------------//
            ViewBag.AllPrimaryImageLinks = await _productService.GetProductsImagesLinks(listProducts);

            //---------- ads ----------------//
            ICollection<ProductAd> productAds = await _productService.GetProductAds();
            ViewBag.ProductAds = productAds;

            //------- discountai ------------//
            var dlvm = await GetDiscountList(listProducts);
            ViewBag.HasDiscountList = dlvm.HasDiscountList;
            ViewBag.DiscountPriceList = dlvm.DiscountPriceList;
            ViewBag.DiscountEndDateList = dlvm.DiscountEndDateList;

            //------- atributai ------------//
            var alvm = await GetAttributeListInCategory(category);
            ViewBag.AttributeValues = alvm.AttributeValues;
            ViewBag.Attributes = alvm.Attributes;
            ViewBag.SelectedAttributeValueName = attributeName;

            //category management
            CategoryListViewModel clvm = new CategoryListViewModel()
            {
                Categories = new SelectList(await _navigationService.GetAllCategories(), "Id", "Name")
            };
            ViewBag.AllCategories = clvm;

            return View("Index", products);
        }

        // Private methods
        private async Task<AttributeListViewModel> GetAttributeListInCategory(Category category)
        {
            var alvm = new AttributeListViewModel
            {
                Attributes = null,
                AttributeValues = null
            };

            if (category != null)
            {
                IList<AttributeValue> attributeValues = null;
                ICollection<Attribute> attributes = new List<Attribute>();

                await Task.Run(async () =>
                {
                    attributeValues = await _attributeService.GetAttributeValuesInCategory(category.Id);

                    foreach (var attrVal in attributeValues)
                    {
                        var attr = _attributeService.FindAttributeById(attrVal.AttributeId);
                        if (!attributes.Contains(attr)) attributes.Add(attr);
                    }
                });

                alvm.Attributes = attributes;
                alvm.AttributeValues = attributeValues;
            }

            return alvm;
        }

        private async Task<DiscountListViewModel> GetDiscountList(ICollection<Product> products)
        {
            ICollection<ProductDiscount> discountList = await _productService.GetAllDiscounts();

            IList<bool> hasDiscountList = new List<bool>();
            IList<decimal?> discountPriceList = new List<decimal?>();
            IList<DateTime?> dicountEndDateList = new List<DateTime?>();

            foreach (var product in products.Select((value, index) => new {Value = value, Index = index}))
            {
                hasDiscountList.Add(false);
                discountPriceList.Add(null);
                dicountEndDateList.Add(null);

                foreach (var discount in discountList)
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
                            await _productService.DeleteDiscount(discount);
                        }

                        break;
                    }
            }

            return new DiscountListViewModel
            {
                HasDiscountList = hasDiscountList,
                DiscountPriceList = discountPriceList,
                DiscountEndDateList = dicountEndDateList
            };
        }

        //DEPRECATED
        /*private async Task<ICollection<string>> GetProductImages(IList<Product> products)
        {
            String[] allPrimaryImageLinks = new String[products.Count];

            for (int i = 0; i < products.Count; i++)
            {
                IList<ProductImage> primaryImage = await _productService.GetProductImages(products[i]);

                if (primaryImage.Count > 0)
                {
                    allPrimaryImageLinks[i] = primaryImage[0].ImageUrl;
                }
                else
                {
                    allPrimaryImageLinks[i] = "product-image-placeholder.jpg";
                }
            }
            return allPrimaryImageLinks;
        }*/

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Search(string searchInput)
        {
            return RedirectToAction("Index", "Home", new {isSearch = true, searchText = searchInput});
        }
    }
}