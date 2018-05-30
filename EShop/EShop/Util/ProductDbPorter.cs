using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using EShop.Data;
using EShop.Models.EFModels.Attribute;
using EShop.Models.EFModels.Category;
using EShop.Models.EFModels.Product;
using FastMember;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using Attribute = EShop.Models.EFModels.Attribute.Attribute;

namespace EShop.Util
{
    public static class ProductDbPorter
    {
        public static bool prettyXls = true;

        public static async Task<byte[]> ExportAsync(ApplicationDbContext context, string productImageFilePath, string attributeImageFilePath, string carouselImagePath)
        {
            if (prettyXls)
                return await PrettyExportAsync(context, productImageFilePath, attributeImageFilePath, carouselImagePath);

            var productsInfo = new ProductsInfo();
            await productsInfo.LoadFromDbContextAsync(context);

            try
            {
                using (var package = new ExcelPackage())
                {
                    package.ProductsInfoToSheets(productsInfo);
                    package.ImagesToSheet("Product image files", productImageFilePath);
                    package.ImagesToSheet("Attribute icon files", attributeImageFilePath);
                    package.ImagesToSheet("Carousel image files", carouselImagePath);

                    return package.GetAsByteArray();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<int> ImportAsync(ApplicationDbContext context, IFormFile file, string productImageFilePath, string attributeImageFilePath, string carouselImagePath)
        {
            if (prettyXls)
                return await PrettyImportAsync(context, file, productImageFilePath, attributeImageFilePath, carouselImagePath);

            ProductsInfo productsInfo;
            try
            {
                using (var package = new ExcelPackage())
                {
                    using (var stream = file.OpenReadStream())
                    {
                        package.Load(stream);
                    }

                    productsInfo = await package.SheetsToProductsInfoAsync();

                    package.SheetToImages("Product image files", productImageFilePath);
                    package.SheetToImages("Attribute icon files", attributeImageFilePath);
                    package.SheetToImages("Carousel image files", carouselImagePath);
                }

                await productsInfo.SaveToDbContextAsync(context);
            }
            catch (Exception)
            {
                return -1;
            }
            return 0;
        }

        public static async Task WipeDbProductsAsync(ApplicationDbContext context)
        {
            await Task.Run(() =>
            {
                context.Attribute.RemoveRange(context.Attribute.ToList());
                context.AttributeValue.RemoveRange(context.AttributeValue.ToList());
                context.Category.RemoveRange(context.Category.ToList());
                context.CategoryCategory.RemoveRange(context.CategoryCategory.ToList());
                context.Product.RemoveRange(context.Product.ToList());
                context.ProductAd.RemoveRange(context.ProductAd.ToList());
                context.ProductAttributeValue.RemoveRange(context.ProductAttributeValue.ToList());
                context.ProductCategory.RemoveRange(context.ProductCategory.ToList());
                context.ProductDiscount.RemoveRange(context.ProductDiscount.ToList());
                context.ProductImage.RemoveRange(context.ProductImage.ToList());
                context.ProductProperty.RemoveRange(context.ProductProperty.ToList());
                context.SaveChanges();
            });
        }

        public static void WipeImages(string productImageFilePath, string attributeImageFilePath, string carouselImagePath)
        {
            var productPath = new DirectoryInfo(productImageFilePath);
            var attributePath = new DirectoryInfo(attributeImageFilePath);
            var carouselPath = new DirectoryInfo(carouselImagePath);

            foreach (var file in productPath.GetFiles())
                if (file.Name != "product-image-placeholder.jpg")
                    file.Delete();
            foreach (var file in attributePath.GetFiles()) file.Delete();
            foreach (var file in carouselPath.GetFiles())
                if (file.Name != "ad-placeholder.png")
                    file.Delete();
        }

        private class ProductsInfo
        {
            private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

            public List<Attribute> Attributes { get; set; }
            public List<AttributeValue> AttributeValues { get; set; }
            public List<Category> Categories { get; set; }
            public List<CategoryCategory> CategoryCategories { get; set; }
            public List<Product> Products { get; set; }
            public List<ProductAd> ProductAds { get; set; }
            public List<ProductAttributeValue> ProductAttributeValues { get; set; }
            public List<ProductCategory> ProductCategories { get; set; }
            public List<ProductDiscount> ProductDiscounts { get; set; }
            public List<ProductImage> ProductImages { get; set; }
            public List<ProductProperty> ProductProperties { get; set; }

            public async Task LoadFromDbContextAsync(ApplicationDbContext context)
            {
                await Task.Run(() =>
                {
                    Attributes = context.Attribute.ToList();
                    AttributeValues = context.AttributeValue.ToList();
                    Categories = context.Category.ToList();
                    CategoryCategories = context.CategoryCategory.ToList();
                    Products = context.Product.ToList();
                    ProductAds = context.ProductAd.ToList();
                    ProductAttributeValues = context.ProductAttributeValue.ToList();
                    ProductCategories = context.ProductCategory.ToList();
                    ProductDiscounts = context.ProductDiscount.ToList();
                    ProductImages = context.ProductImage.ToList();
                    ProductProperties = context.ProductProperty.ToList();
                });
            }

            public async Task SaveToDbContextAsync(ApplicationDbContext context)
            {
                await SemaphoreSlim.WaitAsync();
                try
                {
                    var currentDbState = new ProductsInfo();
                    await currentDbState.LoadFromDbContextAsync(context);

                    var wrappedProductsInfo = new WrappedProductsInfo(this);

                    //LEFT TO ADD: Product, ProductDiscount, ProductAd, ProductImage, ProductAttributeValue, AttributeValue, Attribute, ProductProperty, ProductCategory, Category, CategoryCategory
                    //ADDING IN THIS STEP: Product, Attribute, Categories

                    //Add missing products
                    foreach (var importProduct in Products)
                    {
                        var dbProduct = currentDbState.Products.FirstOrDefault(p =>
                        {
                            return p.Equals(importProduct);
                        });
                        if (dbProduct == null)
                        {
                            importProduct.Id = 0;
                            context.Product.Add(importProduct);
                        }
                    }

                    //Add missing attributes
                    foreach (var importAttribute in Attributes)
                    {
                        var dbAttribute = currentDbState.Attributes.FirstOrDefault(a =>
                        {
                            return a.Equals(importAttribute);
                        });
                        if (dbAttribute == null)
                        {
                            importAttribute.Id = 0;
                            context.Attribute.Add(importAttribute);
                        }
                    }

                    //Add missing categories
                    foreach (var importCategory in Categories)
                    {
                        var dbCategory = currentDbState.Categories.FirstOrDefault(c =>
                        {
                            return c.Equals(importCategory);
                        });
                        if (dbCategory == null)
                        {
                            importCategory.Id = 0;
                            context.Category.Add(importCategory);
                        }
                    }

                    await context.SaveChangesAsync();

                    //Update every entity's ProductId value
                    foreach (var updatedProduct in wrappedProductsInfo.Products)
                    {
                        ProductDiscounts.Where(x => { return x.ProductId == updatedProduct.Item1; }).All(x =>
                        {
                            x.ProductId = updatedProduct.Item2.Id;
                            return true;
                        });
                        ProductAds.Where(x => { return x.ProductId == updatedProduct.Item1; }).All(x =>
                        {
                            x.ProductId = updatedProduct.Item2.Id;
                            return true;
                        });
                        ProductImages.Where(x => { return x.ProductId == updatedProduct.Item1; }).All(x =>
                        {
                            x.ProductId = updatedProduct.Item2.Id;
                            return true;
                        });
                        ProductAttributeValues.Where(x => { return x.ProductId == updatedProduct.Item1; }).All(x =>
                        {
                            x.ProductId = updatedProduct.Item2.Id;
                            return true;
                        });
                        ProductProperties.Where(x => { return x.ProductId == updatedProduct.Item1; }).All(x =>
                        {
                            x.ProductId = updatedProduct.Item2.Id;
                            return true;
                        });
                        ProductCategories.Where(x => { return x.ProductId == updatedProduct.Item1; }).All(x =>
                        {
                            x.ProductId = updatedProduct.Item2.Id;
                            return true;
                        });
                    }

                    //Update every entity's AttributeId values
                    foreach (var updatedAttribute in wrappedProductsInfo.Attributes)
                        AttributeValues.Where(x => { return x.AttributeId == updatedAttribute.Item1; }).All(x =>
                        {
                            x.AttributeId = updatedAttribute.Item2.Id;
                            return true;
                        });

                    //Update every entity's CategoryId and ParentCategoryId values
                    foreach (var updatedCategory in wrappedProductsInfo.Categories)
                    {
                        ProductCategories.Where(x => { return x.CategoryId == updatedCategory.Item1; }).All(x =>
                        {
                            x.CategoryId = updatedCategory.Item2.Id;
                            return true;
                        });
                        CategoryCategories.Where(x => { return x.CategoryId == updatedCategory.Item1; }).All(x =>
                        {
                            x.CategoryId = updatedCategory.Item2.Id;
                            return true;
                        });
                        CategoryCategories.Where(x => { return x.ParentCategoryId == updatedCategory.Item1; }).All(x =>
                        {
                            x.ParentCategoryId = updatedCategory.Item2.Id;
                            return true;
                        });
                    }

                    //LEFT TO ADD: ProductDiscount, ProductAd, ProductImage, ProductAttributeValue, AttributeValue, ProductProperty, ProductCategory, CategoryCategory
                    //ADDING IN THIS STEP: ProductDiscount, ProductAd, ProductImage, AttributeValue, ProductProperty, CategoryCategory, ProductCategory

                    //Add missing product discounts
                    foreach (var importProductDiscount in ProductDiscounts)
                    {
                        var dbProductDiscount = currentDbState.ProductDiscounts.FirstOrDefault(pd =>
                        {
                            return pd.Equals(importProductDiscount);
                        });
                        if (dbProductDiscount == null)
                        {
                            importProductDiscount.Id = 0;
                            context.ProductDiscount.Add(importProductDiscount);
                        }
                    }

                    //Add missing product ads
                    foreach (var importProductAd in ProductAds)
                    {
                        var dbProductAd = currentDbState.ProductAds.FirstOrDefault(pa =>
                        {
                            return pa.Equals(importProductAd);
                        });
                        if (dbProductAd == null)
                        {
                            importProductAd.Id = 0;
                            context.ProductAd.Add(importProductAd);
                        }
                    }

                    //Add missing product images
                    foreach (var importProductImage in ProductImages)
                    {
                        var dbProductImage = currentDbState.ProductImages.FirstOrDefault(pi =>
                        {
                            return pi.Equals(importProductImage);
                        });
                        if (dbProductImage == null)
                        {
                            importProductImage.Id = 0;
                            context.ProductImage.Add(importProductImage);
                        }
                    }

                    //Add missing product properties
                    foreach (var importProductProperty in ProductProperties)
                    {
                        var dbProductProperty = currentDbState.ProductProperties.FirstOrDefault(pp =>
                        {
                            return pp.Equals(importProductProperty);
                        });
                        if (dbProductProperty == null)
                        {
                            importProductProperty.Id = 0;
                            context.ProductProperty.Add(importProductProperty);
                        }
                    }

                    //Add missing attribute values
                    foreach (var importAttributeValue in AttributeValues)
                    {
                        var dbAttributeValue = currentDbState.AttributeValues.FirstOrDefault(av =>
                        {
                            return av.Equals(importAttributeValue);
                        });
                        if (dbAttributeValue == null)
                        {
                            importAttributeValue.Id = 0;
                            context.AttributeValue.Add(importAttributeValue);
                        }
                    }

                    //Add missing category categories
                    foreach (var importCategoryCategory in CategoryCategories)
                    {
                        var dbCategoryCategory = currentDbState.CategoryCategories.FirstOrDefault(cc =>
                        {
                            return cc.Equals(importCategoryCategory);
                        });
                        if (dbCategoryCategory == null)
                        {
                            importCategoryCategory.Id = 0;
                            context.CategoryCategory.Add(importCategoryCategory);
                        }
                    }

                    //Add missing product categories
                    foreach (var importProductCategory in ProductCategories)
                    {
                        var dbProductCategory = currentDbState.ProductCategories.FirstOrDefault(pc =>
                        {
                            return pc.Equals(importProductCategory);
                        });
                        if (dbProductCategory == null)
                        {
                            importProductCategory.Id = 0;
                            context.ProductCategory.Add(importProductCategory);
                        }
                    }

                    await context.SaveChangesAsync();

                    //Update every entity's AttributeValueId
                    foreach (var updatedAttributeValue in wrappedProductsInfo.AttributeValues)
                        ProductAttributeValues.Where(x => { return x.AttributeValueId == updatedAttributeValue.Item1; })
                            .All(x =>
                            {
                                x.AttributeValueId = updatedAttributeValue.Item2.Id;
                                return true;
                            });

                    //LEFT TO ADD: ProductAttributeValue
                    //ADDING IN THIS STEP: ProductAttributeValue

                    //Add missing product attribute values
                    foreach (var importProductAttributeValue in ProductAttributeValues)
                    {
                        var dbProductAttributeValue = currentDbState.ProductAttributeValues.FirstOrDefault(pav =>
                        {
                            return pav.Equals(importProductAttributeValue);
                        });
                        if (dbProductAttributeValue == null)
                        {
                            importProductAttributeValue.Id = 0;
                            context.ProductAttributeValue.Add(importProductAttributeValue);
                        }
                    }


                    await context.SaveChangesAsync();
                }
                finally
                {
                    SemaphoreSlim.Release();
                }
            }

            public class WrappedProductsInfo
            {
                public WrappedProductsInfo(ProductsInfo productsInfo)
                {
                    Load(productsInfo);
                }

                public List<Tuple<int, Attribute>> Attributes { get; set; }
                public List<Tuple<int, AttributeValue>> AttributeValues { get; set; }
                public List<Tuple<int, Category>> Categories { get; set; }
                public List<Tuple<int, CategoryCategory>> CategoryCategories { get; set; }
                public List<Tuple<int, Product>> Products { get; set; }
                public List<Tuple<int, ProductAd>> ProductAds { get; set; }
                public List<Tuple<int, ProductAttributeValue>> ProductAttributeValues { get; set; }
                public List<Tuple<int, ProductCategory>> ProductCategories { get; set; }
                public List<Tuple<int, ProductDiscount>> ProductDiscounts { get; set; }
                public List<Tuple<int, ProductImage>> ProductImages { get; set; }
                public List<Tuple<int, ProductProperty>> ProductProperties { get; set; }

                private void Load(ProductsInfo productsInfo)
                {
                    Attributes = productsInfo.Attributes.Select(a => { return new Tuple<int, Attribute>(a.Id, a); })
                        .ToList();
                    AttributeValues = productsInfo.AttributeValues.Select(av =>
                    {
                        return new Tuple<int, AttributeValue>(av.Id, av);
                    }).ToList();
                    Categories = productsInfo.Categories.Select(c => { return new Tuple<int, Category>(c.Id, c); })
                        .ToList();
                    CategoryCategories = productsInfo.CategoryCategories.Select(cc =>
                    {
                        return new Tuple<int, CategoryCategory>(cc.Id, cc);
                    }).ToList();
                    Products = productsInfo.Products.Select(p => { return new Tuple<int, Product>(p.Id, p); }).ToList();
                    ProductAds = productsInfo.ProductAds.Select(pa => { return new Tuple<int, ProductAd>(pa.Id, pa); })
                        .ToList();
                    ProductAttributeValues = productsInfo.ProductAttributeValues.Select(pav =>
                    {
                        return new Tuple<int, ProductAttributeValue>(pav.Id, pav);
                    }).ToList();
                    ProductCategories = productsInfo.ProductCategories.Select(pc =>
                    {
                        return new Tuple<int, ProductCategory>(pc.Id, pc);
                    }).ToList();
                    ProductDiscounts = productsInfo.ProductDiscounts.Select(pd =>
                    {
                        return new Tuple<int, ProductDiscount>(pd.Id, pd);
                    }).ToList();
                    ProductImages = productsInfo.ProductImages.Select(pi =>
                    {
                        return new Tuple<int, ProductImage>(pi.Id, pi);
                    }).ToList();
                    ProductProperties = productsInfo.ProductProperties.Select(pp =>
                    {
                        return new Tuple<int, ProductProperty>(pp.Id, pp);
                    }).ToList();
                }
            }
        }

        public static async Task<byte[]> PrettyExportAsync(ApplicationDbContext context, string productImageFilePath, string attributeImageFilePath, string carouselImagePath)
        {
            var productsInfo = new ProductsInfo();
            await productsInfo.LoadFromDbContextAsync(context);
            try
            {
                using (var package = new ExcelPackage())
                {
                    package.Workbook.Worksheets.Add("Sheet1");
                    var sheet = package.Workbook.Worksheets["Sheet1"];
                    List<string[]> table = new List<string[]>();
                    table.Add(new string[] { "Product Name", "Description", "Price", "Discount Price", "Discount Ends", "Ad Banner", "Primary Image", "Secondary Images", "Properties", "Categories", "Attributes" });

                    foreach (var product in productsInfo.Products)
                    {
                        List<string> row = new List<string>();
                        row.Add(product.Name);
                        row.Add(product.Description);
                        row.Add(product.Price.ToString());
                        row.Add(product.ProductDiscount == null ? "" : product.ProductDiscount.DiscountPrice.ToString());
                        row.Add(product.ProductDiscount == null ? "" : product.ProductDiscount.Ends.ToString("G"));
                        row.Add(product.ProductAd == null ? "" : product.ProductAd.AdImageUrl);

                        bool primaryAdded = false;
                        if (product.ProductImages != null)
                        {
                            foreach (var productImage in product.ProductImages)
                            {
                                if (productImage.IsPrimary)
                                {
                                    row.Add(productImage.ImageUrl);
                                    primaryAdded = true;
                                    break;
                                }
                            }
                        }
                        if (!primaryAdded)
                            row.Add("");

                        StringBuilder sb = new StringBuilder();
                        if (product.ProductImages != null)
                        {
                            foreach (var productImage in product.ProductImages)
                                if (!productImage.IsPrimary)
                                    sb.Append(productImage.ImageUrl).Append(", ");
                            if (sb.Length > 0)
                                sb.Length -= 2;
                        }
                        row.Add(sb.ToString());
                        sb.Length = 0;

                        if (product.ProductProperies != null)
                        {
                            foreach (var productProperty in product.ProductProperies)
                                sb.Append('{').Append(productProperty.Name).Append(": ").Append(productProperty.Description).Append("}, ");
                            if (sb.Length > 0)
                                sb.Length -= 2;
                        }
                        row.Add(sb.ToString());
                        sb.Length = 0;

                        if (product.ProductCategories != null)
                        {
                            foreach (var productCategory in product.ProductCategories)
                            {
                                Category dbCategory = productCategory.Category;
                                CategoryCategory dbCategoryCategory = productCategory.Category.CategoryCategory;
                                string category = dbCategory.Name + "(" + dbCategory.Description + ")";
                                while (dbCategoryCategory != null && dbCategoryCategory.ParentCategory != null)
                                {
                                    dbCategory = dbCategoryCategory.ParentCategory;
                                    dbCategoryCategory = dbCategory.CategoryCategory;
                                    category = dbCategory.Name + "(" + dbCategory.Description + ")" + "/" + category;
                                }
                                category += ", ";
                                sb.Append(category);
                            }
                            if (sb.Length > 0)
                                sb.Length -= 2;
                        }
                        row.Add(sb.ToString());
                        sb.Length = 0;

                        if (product.ProductAttributeValues != null)
                        {
                            foreach (var productAttributeValue in product.ProductAttributeValues)
                            {
                                sb.Append('{').Append(productAttributeValue.AttributeValue.Attribute.Name).Append("(" + productAttributeValue.AttributeValue.Attribute.IconUrl + ")").Append(": ").Append(productAttributeValue.AttributeValue.Name).Append("}, ");
                            }
                            if (sb.Length > 0)
                                sb.Length -= 2;
                        }
                        row.Add(sb.ToString());

                        table.Add(row.ToArray());
                    }

                    sheet.Cells["A1"].LoadFromArrays(table);
                    sheet.Row(1).Style.Font.Bold = true;
                    var cells = sheet.Cells[sheet.Dimension.Start.Row, sheet.Dimension.Start.Column, sheet.Dimension.End.Row, sheet.Dimension.End.Column];
                    cells.Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    cells.Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    cells.Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    cells.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    for (int i = 1; i <= table[0].Length; i++)
                    {
                        sheet.Column(i).AutoFit(8.43, 84.3);
                        sheet.Column(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        sheet.Column(i).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        sheet.Column(i).Style.WrapText = true;
                    }
                    return package.GetAsByteArray();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<int> PrettyImportAsync(ApplicationDbContext context, IFormFile file, string productImageFilePath, string attributeImageFilePath, string carouselImagePath)
        {
            int i = -1;
            try
            {
                using (var package = new ExcelPackage())
                {
                    using (var stream = file.OpenReadStream())
                    {
                        package.Load(stream);
                    }
                    var sheet = package.Workbook.Worksheets["Sheet1"];
                    var currentDbState = new ProductsInfo();
                    await currentDbState.LoadFromDbContextAsync(context);
                    i = sheet.Dimension.Start.Row + 1;
                    for (; i <= sheet.Dimension.End.Row; i++)
                    {
                        //if (sheet.Cells[i, 1].Value == null)
                        //    continue;
                        Product product = new Product()
                        {
                            Name = sheet.Cells[i, 1].Value.ToString().Trim(),
                            Description = sheet.Cells[i, 2].Value.ToString().Trim(),
                            Price = decimal.Parse(sheet.Cells[i, 3].Value.ToString().Trim())
                        };
                        var dbProduct = currentDbState.Products.FirstOrDefault(p =>
                        {
                            return p.Equals(product);
                        });
                        if (dbProduct == null)
                        {
                            context.Product.Add(product);
                            currentDbState.Products.Add(product);
                            dbProduct = product;
                            await context.SaveChangesAsync();
                        }

                        if (sheet.Cells[i, 4].Value != null && sheet.Cells[i, 5].Value != null)
                        {
                            Console.WriteLine(DateTime.Parse(sheet.Cells[i, 5].Value.ToString().Trim()));
                            ProductDiscount discount = new ProductDiscount()
                            {
                                DiscountPrice = decimal.Parse(sheet.Cells[i, 4].Value.ToString().Trim()),
                                Ends = DateTime.Parse(sheet.Cells[i, 5].Value.ToString().Trim()),
                                ProductId = dbProduct.Id
                            };
                            var dbDiscount = currentDbState.ProductDiscounts.FirstOrDefault(pd =>
                            {
                                return pd.Equals(discount);
                            });
                            if (dbDiscount == null)
                            {
                                context.ProductDiscount.Add(discount);
                                currentDbState.ProductDiscounts.Add(discount);
                                dbDiscount = discount;
                            }
                        }

                        if (sheet.Cells[i, 6].Value != null)
                        {
                            ProductAd ad = new ProductAd()
                            {
                                AdImageUrl = sheet.Cells[i, 6].Value.ToString().Trim(),
                                ProductId = dbProduct.Id
                            };
                            var dbAd = currentDbState.ProductAds.FirstOrDefault(pa =>
                            {
                                return pa.Equals(ad);
                            });
                            if (dbAd == null)
                            {
                                context.ProductAd.Add(ad);
                                currentDbState.ProductAds.Add(ad);
                                dbAd = ad;
                            }
                        }

                        if (sheet.Cells[i, 7].Value != null)
                        {
                            ProductImage image = new ProductImage()
                            {
                                IsPrimary = true,
                                ImageUrl = sheet.Cells[i, 7].Value.ToString().Trim(),
                                ProductId = dbProduct.Id
                            };
                            var dbImage = currentDbState.ProductImages.FirstOrDefault(pi =>
                            {
                                return pi.Equals(image);
                            });
                            if (dbImage == null)
                            {
                                context.ProductImage.Add(image);
                                currentDbState.ProductImages.Add(image);
                                dbImage = image;
                            }
                        }

                        if (sheet.Cells[i, 8].Value != null)
                        {
                            foreach (var imageUrl in sheet.Cells[i, 8].Value.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries))
                            {
                                ProductImage image = new ProductImage()
                                {
                                    IsPrimary = false,
                                    ImageUrl = imageUrl.Trim(),
                                    ProductId = dbProduct.Id
                                };
                                var dbImage = currentDbState.ProductImages.FirstOrDefault(pi =>
                                {
                                    return pi.Equals(image);
                                });
                                if (dbImage == null)
                                {
                                    context.ProductImage.Add(image);
                                    currentDbState.ProductImages.Add(image);
                                    dbImage = image;
                                }
                            }
                        }

                        if (sheet.Cells[i, 9].Value != null)
                        {
                            foreach (Match propertyMatch in Regex.Matches(sheet.Cells[i, 9].Value.ToString(), @"{.+?}"))
                            {
                                string propertyValue = propertyMatch.Value;
                                ProductProperty property = new ProductProperty()
                                {
                                    Name = propertyValue.Substring(1, propertyValue.IndexOf(':') - 1).Trim(),
                                    Description = propertyValue.Substring(propertyValue.IndexOf(':') + 1, propertyValue.Length - propertyValue.IndexOf(':') - 2).Trim(),
                                    ProductId = dbProduct.Id
                                };
                                var dbProperty = currentDbState.ProductProperties.FirstOrDefault(pp =>
                                {
                                    return pp.Equals(property);
                                });
                                if (dbProperty == null)
                                {
                                    context.ProductProperty.Add(property);
                                    currentDbState.ProductProperties.Add(property);
                                    dbProperty = property;
                                }
                            }
                        }

                        if (sheet.Cells[i, 10].Value != null)
                        {
                            foreach (var categoriesStr in sheet.Cells[i, 10].Value.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries))
                            {
                                Category lastDbCategory = null;
                                foreach (var categoryStr in categoriesStr.Split('/'))
                                {
                                    string name = categoryStr.Substring(0, categoryStr.IndexOf('('));
                                    string desc = categoryStr.Substring(categoryStr.IndexOf('(') + 1, categoryStr.IndexOf(')') - categoryStr.IndexOf('(') - 1);
                                    Category category = new Category()
                                    {
                                        Name = name.Trim(),
                                        Description = desc.Trim(),
                                    };
                                    var dbCategory = currentDbState.Categories.FirstOrDefault(c =>
                                    {
                                        return c.Equals(category);
                                    });
                                    if (dbCategory == null)
                                    {
                                        context.Category.Add(category);
                                        currentDbState.Categories.Add(category);
                                        dbCategory = category;
                                        await context.SaveChangesAsync();
                                    }
                                    
                                    if (lastDbCategory != null)
                                    {
                                        CategoryCategory categoryCategory = new CategoryCategory()
                                        {
                                            CategoryId = dbCategory.Id,
                                            ParentCategoryId = lastDbCategory.Id
                                        };
                                        var dbCategoryCategory = currentDbState.CategoryCategories.FirstOrDefault(cc =>
                                        {
                                            return cc.Equals(categoryCategory);
                                        });
                                        if (dbCategoryCategory == null)
                                        {
                                            context.CategoryCategory.Add(categoryCategory);
                                            currentDbState.CategoryCategories.Add(categoryCategory);
                                            dbCategoryCategory = categoryCategory;
                                        }
                                    }
                                    lastDbCategory = dbCategory;
                                }
                                if (lastDbCategory != null)
                                {
                                    ProductCategory productCategory = new ProductCategory()
                                    {
                                        ProductId = dbProduct.Id,
                                        CategoryId = lastDbCategory.Id
                                    };
                                    var dbProductCategory = currentDbState.ProductCategories.FirstOrDefault(pc =>
                                    {
                                        return pc.Equals(productCategory);
                                    });
                                    if (dbProductCategory == null)
                                    {
                                        context.ProductCategory.Add(productCategory);
                                        currentDbState.ProductCategories.Add(productCategory);
                                        dbProductCategory = productCategory;
                                        await context.SaveChangesAsync();
                                    }
                                }
                            }
                        }

                        if (sheet.Cells[i, 11].Value != null)
                        {
                            foreach (Match attributeMatch in Regex.Matches(sheet.Cells[i, 11].Value.ToString(), @"{.+?}"))
                            {
                                string attributeStr = attributeMatch.Value;
                                attributeStr = attributeStr.Substring(1, attributeStr.Length - 2);
                                string attributeName = attributeStr.Substring(0, attributeStr.IndexOf('('));
                                string attributeIconUrl = attributeStr.Substring(attributeStr.IndexOf('(') + 1, attributeStr.IndexOf(')', attributeStr.IndexOf('(')) - attributeStr.IndexOf('(') - 1);
                                string attributeValueName = attributeStr.Substring(attributeStr.IndexOf(':') + 1);

                                Attribute attribute = new Attribute()
                                {
                                    Name = attributeName,
                                    IconUrl = attributeIconUrl
                                };
                                var dbAttribute = currentDbState.Attributes.FirstOrDefault(a =>
                                {
                                    return a.Equals(attribute);
                                });
                                if (dbAttribute == null)
                                {
                                    context.Attribute.Add(attribute);
                                    currentDbState.Attributes.Add(attribute);
                                    dbAttribute = attribute;
                                    await context.SaveChangesAsync();
                                }

                                AttributeValue attributeValue = new AttributeValue()
                                {
                                    Name = attributeValueName,
                                    AttributeId = dbAttribute.Id
                                };
                                var dbAttributeValue = currentDbState.AttributeValues.FirstOrDefault(av =>
                                {
                                    return av.Equals(attributeValue);
                                });
                                if (dbAttributeValue == null)
                                {
                                    context.AttributeValue.Add(attributeValue);
                                    currentDbState.AttributeValues.Add(attributeValue);
                                    dbAttributeValue = attributeValue;
                                    await context.SaveChangesAsync();
                                }

                                ProductAttributeValue productAttributeValue = new ProductAttributeValue()
                                {
                                    AttributeValueId = dbAttributeValue.Id,
                                    ProductId = dbProduct.Id
                                };
                                var dbProductAttributeValue = currentDbState.ProductAttributeValues.FirstOrDefault(pav =>
                                {
                                    return pav.Equals(productAttributeValue);
                                });
                                if (dbProductAttributeValue == null)
                                {
                                    context.ProductAttributeValue.Add(productAttributeValue);
                                    currentDbState.ProductAttributeValues.Add(productAttributeValue);
                                    dbProductAttributeValue = productAttributeValue;
                                }
                            }
                        }
                    }
                }
                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                return i;
            }
            return 0;
        }

        #region Export methods

        private static void ProductsInfoToSheets(this ExcelPackage package, ProductsInfo data)
        {
            package.AddToSheets("Attributes", data.Attributes);
            package.AddToSheets("AttributeValues", data.AttributeValues);
            package.AddToSheets("Categories", data.Categories);
            package.AddToSheets("CategoryCategories", data.CategoryCategories);
            package.AddToSheets("Products", data.Products);
            package.AddToSheets("ProductAds", data.ProductAds);
            package.AddToSheets("ProductAttributeValues", data.ProductAttributeValues);
            package.AddToSheets("ProductCategories", data.ProductCategories);
            package.AddToSheets("ProductDiscounts", data.ProductDiscounts);
            package.AddToSheets("ProductImages", data.ProductImages);
            package.AddToSheets("ProductProperties", data.ProductProperties);
        }

        private static void AddToSheets<T>(this ExcelPackage package, string sheetName, List<T> data)
        {
            var table = new DataTable();
            using (var reader = ObjectReader.Create(data))
            {
                table.Load(reader);
            }

            // Stripping redundant columns (commented out line that would include RowVersion in the serialization)
            var columnsToRemove = new Stack<DataColumn>();
            foreach (DataColumn column in table.Columns)
            {
                var methodInfo = column.DataType.GetMethod("ToString", new Type[0]);
                if (methodInfo == null ||
                    methodInfo.DeclaringType == typeof(object) /*&& column.DataType != typeof(byte[])*/
                ) columnsToRemove.Push(column);
            }

            while (columnsToRemove.Count > 0)
            {
                var column = columnsToRemove.Pop();
                table.Columns.Remove(column);
            }


            var worksheet = package.Workbook.Worksheets.Add(sheetName);
            worksheet.Cells["A1"].LoadFromDataTable(table, true);

            // Fix for date time vars
            var colNumber = 1;
            foreach (DataColumn column in table.Columns)
            {
                if (column.DataType == typeof(DateTime))
                    worksheet.Column(colNumber).Style.Numberformat.Format = "yyyy-MM-dd hh:mm:ss";
                colNumber++;
            }

            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        private static void ImagesToSheet(this ExcelPackage package, string sheetName, string imagesDirectory)
        {
            var worksheet = package.Workbook.Worksheets.Add(sheetName);

            var imagePaths = Directory.GetFiles(imagesDirectory);

            for (var i = 0; i < imagePaths.Length; i++)
            {
                var image = Image.FromFile(imagePaths[i]);
                var picture = worksheet.Drawings.AddPicture(Path.GetFileName(imagePaths[i]), image);
                picture.SetPosition(i, 0, 0, 0);
                worksheet.Row(i + 1).Height = image.Height * 0.76d; // Tweaked to match pixel to excel measurements
            }
        }

        #endregion

        #region Import methods

        private static async Task<ProductsInfo> SheetsToProductsInfoAsync(this ExcelPackage package)
        {
            return await Task.Run(() =>
            {
                var info = new ProductsInfo();
                var worksheets = package.Workbook.Worksheets;
                info.Attributes = worksheets["Attributes"].GetListFromWorksheet<Attribute>();
                info.AttributeValues = worksheets["AttributeValues"].GetListFromWorksheet<AttributeValue>();
                info.Categories = worksheets["Categories"].GetListFromWorksheet<Category>();
                info.CategoryCategories = worksheets["CategoryCategories"].GetListFromWorksheet<CategoryCategory>();
                info.Products = worksheets["Products"].GetListFromWorksheet<Product>();
                info.ProductAds = worksheets["ProductAds"].GetListFromWorksheet<ProductAd>();
                info.ProductAttributeValues =
                    worksheets["ProductAttributeValues"].GetListFromWorksheet<ProductAttributeValue>();
                info.ProductCategories = worksheets["ProductCategories"].GetListFromWorksheet<ProductCategory>();
                info.ProductDiscounts = worksheets["ProductDiscounts"].GetListFromWorksheet<ProductDiscount>();
                info.ProductImages = worksheets["ProductImages"].GetListFromWorksheet<ProductImage>();
                info.ProductProperties = worksheets["ProductProperties"].GetListFromWorksheet<ProductProperty>();
                return info;
            });
        }

        private static List<T> GetListFromWorksheet<T>(this ExcelWorksheet worksheet) where T : class, new()
        {
            var table = new DataTable();
            foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                table.Columns.Add(firstRowCell.Text);

            for (var rowNum = 2; rowNum <= worksheet.Dimension.End.Row; rowNum++)
            {
                var worksheetRow = worksheet.Cells[rowNum, 1, rowNum, worksheet.Dimension.End.Column];
                var row = table.Rows.Add();
                foreach (var cell in worksheetRow) row[cell.Start.Column - 1] = cell.Text;
            }

            return table.ToList<T>();
        }


        private static List<T> ToList<T>(this DataTable table) where T : class, new()
        {
            var array = new T[table.Rows.Count];

            var type = typeof(T);
            var propInfo = type.GetProperties();
            var accessor = TypeAccessor.Create(type);

            for (var v = 0; v < table.Rows.Count; v++)
            {
                var row = table.Rows[v];
                var entity = new T();

                for (var i = 0; i < propInfo.Length; i++)
                {
                    var prop = propInfo[i];
                    try
                    {
                        accessor[entity, prop.Name] = Convert.ChangeType(row[prop.Name], prop.PropertyType);
                    }
                    catch
                    {
                        //Hacky bypass non string and int bugs
                        try
                        {
                            if (prop.PropertyType == typeof(bool))
                                accessor[entity, prop.Name] = row[prop.Name].ToString() == "1";
                            else
                                accessor[entity, prop.Name] = int.Parse(row[prop.Name].ToString());
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }

                array[v] = entity;
            }

            return array.ToList();
        }

        private static void SheetToImages(this ExcelPackage package, string sheetName, string imagesDirectory)
        {
            var drawings = package.Workbook.Worksheets[sheetName].Drawings;

            Parallel.For(0, drawings.Count, i =>
            {
                var image = drawings[i] as ExcelPicture;
                if (image != null)
                {
                    var newPath = Path.Combine(imagesDirectory, image.Name);
                    image.Image.Save(newPath);
                }
            });
        }

        #endregion
    }
}