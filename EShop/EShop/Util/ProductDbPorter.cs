using EShop.Data;
using EShop.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Util
{
    public static class ProductDbPorter
    {
        private class ProductsInfo
        {
            public List<Models.Attribute> Attributes { get; set; }
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

            public void LoadFromDbContext(ApplicationDbContext context)
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
                RemoveNonDbColumns();
            }

            private void RemoveNonDbColumns()
            {
                foreach (var attribute in Attributes)
                {
                    attribute.AttributeValues = null;
                }
                foreach (var attributeValue in AttributeValues)
                {
                    attributeValue.Attribute = null;
                    attributeValue.ProductAttributeValues = null;
                }
                foreach (var category in Categories)
                {
                    category.ProductCategories = null;
                    category.CategoryCategories = null;
                }
                foreach (var categoryCategory in CategoryCategories)
                {
                    categoryCategory.Category = null;
                    categoryCategory.ParentCategory = null;
                }
                foreach (var product in Products)
                {
                    product.ProductDiscount = null;
                    product.ProductAds = null;
                    product.ProductImages = null;
                    product.ProductAttributeValues = null;
                    product.ProductProperties = null;
                    product.ProductCategories = null;
                }
                foreach (var productAd in ProductAds)
                {
                    productAd.Product = null;
                }
                foreach (var productAttributeValue in ProductAttributeValues)
                {
                    productAttributeValue.AttributeValue = null;
                    productAttributeValue.Product = null;
                }
                foreach (var productCategory in ProductCategories)
                {
                    productCategory.Product = null;
                    productCategory.Category = null;
                }
                foreach (var productDiscount in ProductDiscounts)
                {
                    productDiscount.Product = null;
                }
                foreach (var productImage in ProductImages)
                {
                    productImage.Product = null;
                }
                foreach (var productProperty in ProductProperties)
                {
                    productProperty.Product = null;
                }
            }

            public async Task SaveToDbContextAsync(ApplicationDbContext context)
            {
                ProductsInfo currentDbState = new ProductsInfo();
                currentDbState.LoadFromDbContext(context);

                foreach (var importProduct in Products)
                {
                    Product dbProduct = currentDbState.Products.FirstOrDefault((p) => { return p.Equals(importProduct); });
                    if (dbProduct == null)
                    {
                        await context.Product.AddAsync(importProduct);
                        await context.SaveChangesAsync(); //mandatory
                        dbProduct = importProduct;
                    }

                    //Discount
                    foreach (var importDiscount in ProductDiscounts.Where((d) => { return d.ProductId == importProduct.Id; }))
                    {
                        ProductDiscount dbDiscount = currentDbState.ProductDiscounts.FirstOrDefault((d) => { return d.ProductId == dbProduct.Id && d.Equals(importDiscount); });
                        if (dbDiscount == null)
                        {
                            importDiscount.ProductId = dbProduct.Id;
                            await context.ProductDiscount.AddAsync(importDiscount);
                            //await context.SaveChangesAsync(); //optional
                        }
                    }

                    //Ad
                    foreach (var importAd in ProductAds.Where((a) => { return a.ProductId == importProduct.Id; }))
                    {
                        ProductAd dbAd = currentDbState.ProductAds.FirstOrDefault((a) => { return a.ProductId == dbProduct.Id && a.Equals(importAd); });
                        if (dbAd == null)
                        {
                            importAd.ProductId = dbProduct.Id;
                            await context.ProductAd.AddAsync(importAd);
                            //await context.SaveChangesAsync(); //optional
                        }
                    }

                    //ProductImage
                    foreach (var importImage in ProductImages.Where((i) => { return i.ProductId == importProduct.Id; }))
                    {
                        ProductImage dbImage = currentDbState.ProductImages.FirstOrDefault((i) => { return i.ProductId == dbProduct.Id && i.Equals(importImage); });
                        if (dbImage == null)
                        {
                            importImage.ProductId = dbProduct.Id;
                            await context.ProductImage.AddAsync(importImage);
                            //await context.SaveChangesAsync(); //optional
                        }
                    }

                    //ProductAttributeValue
                    foreach (var importProductAttributeValue in ProductAttributeValues.Where((pav) => { return pav.ProductId == importProduct.Id; }))
                    {
                        ProductAttributeValue dbProductAttributeValue = currentDbState.ProductAttributeValues.FirstOrDefault((pav) => { return pav.ProductId == dbProduct.Id && pav.Equals(importProductAttributeValue); });
                        if (dbProductAttributeValue == null)
                        {
                            importProductAttributeValue.ProductId = dbProduct.Id;
                            await context.ProductAttributeValue.AddAsync(importProductAttributeValue);
                            //await context.SaveChangesAsync(); //optional
                            dbProductAttributeValue = importProductAttributeValue;
                        }

                        //AttributeValue
                        foreach (var importAttributeValue in AttributeValues.Where((av) => { return av.Id == importProductAttributeValue.AttributeValueId; }))
                        {
                            AttributeValue dbAttributeValue = currentDbState.AttributeValues.FirstOrDefault((av) => { return av.Id == dbProductAttributeValue.AttributeValueId && av.Equals(importAttributeValue); });
                            if (dbAttributeValue == null)
                            {
                                await context.AttributeValue.AddAsync(importAttributeValue);
                                await context.SaveChangesAsync(); //mandatory
                                dbAttributeValue = importAttributeValue;
                                dbProductAttributeValue.AttributeValueId = dbAttributeValue.Id;
                            }

                            //Attribute
                            foreach (var importAttribute in Attributes.Where((a) => { return a.Id == importAttributeValue.AttributeId; }))
                            {
                                Models.Attribute dbAttribute = currentDbState.Attributes.FirstOrDefault((a) => { return a.Id == dbAttributeValue.AttributeId && a.Equals(importAttribute); });
                                if (dbAttribute == null)
                                {
                                    await context.Attribute.AddAsync(importAttribute);
                                    await context.SaveChangesAsync(); //mandatory
                                    dbAttribute = importAttribute;
                                    dbAttributeValue.AttributeId = dbAttribute.Id;
                                }
                            }
                        }
                    }

                    //ProductProperty
                    foreach (var importProperty in ProductProperties.Where((p) => { return p.ProductId == importProduct.Id; }))
                    {
                        ProductProperty dbProperty = currentDbState.ProductProperties.FirstOrDefault((p) => { return p.ProductId == dbProduct.Id && p.Equals(importProperty); });
                        if (dbProperty == null)
                        {
                            importProperty.ProductId = dbProduct.Id;
                            await context.ProductProperty.AddAsync(importProperty);
                            //await context.SaveChangesAsync(); //optional
                        }
                    }

                    //ProductCategory
                    foreach (var importProductCategory in ProductCategories.Where((pc) => { return pc.ProductId == importProduct.Id; }))
                    {
                        ProductCategory dbProductCategory = currentDbState.ProductCategories.FirstOrDefault((pc) => { return pc.ProductId == dbProduct.Id && pc.Equals(importProductCategory); });
                        if (dbProductCategory == null)
                        {
                            importProductCategory.ProductId = dbProduct.Id;
                            await context.ProductCategory.AddAsync(importProductCategory);
                            //await context.SaveChangesAsync(); //optional
                            dbProductCategory = importProductCategory;
                        }

                        //Category
                        foreach (var importCategory in Categories.Where((c) => { return c.Id == importProductCategory.CategoryId; }))
                        {
                            Category dbCategory = currentDbState.Categories.FirstOrDefault((c) => { return c.Id == dbProductCategory.CategoryId && c.Equals(importCategory); });
                            if (dbCategory == null)
                            {
                                await context.Category.AddAsync(importCategory);
                                await context.SaveChangesAsync(); //mandatory
                                dbCategory = importCategory;
                                dbProductCategory.CategoryId = dbCategory.Id;
                            }

                            //Update affected CategoryCategory ids
                            foreach (var importCategoryCategory in CategoryCategories.Where((cc) => { return cc.CategoryId == importCategory.Id; }))
                                importCategoryCategory.CategoryId = dbCategory.Id;
                            foreach (var importCategoryCategory in CategoryCategories.Where((cc) => { return cc.ParentCategoryId == importCategory.Id; }))
                                importCategoryCategory.ParentCategoryId = dbCategory.Id;
                        }
                    }
                }
                //CategoryCategory
                foreach (var importCategoryCategory in CategoryCategories)
                {
                    if (!currentDbState.CategoryCategories.Any((cc) => { return cc.CategoryId == importCategoryCategory.CategoryId && cc.ParentCategoryId == importCategoryCategory.ParentCategoryId; }))
                    {
                        await context.CategoryCategory.AddAsync(importCategoryCategory);
                    }
                }
                await context.SaveChangesAsync(); //mandatory
            }
        }

        public static bool Export(ApplicationDbContext context, string filePath)
        {
            //filePath = AppDomain.CurrentDomain.BaseDirectory + '\\' + "serialized.txt";

            ProductsInfo productsInfo = new ProductsInfo();
            productsInfo.LoadFromDbContext(context);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            string serialized = JsonConvert.SerializeObject(productsInfo, Formatting.Indented, settings);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                using (var writer = File.CreateText(filePath))
                    writer.Write(serialized);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static async Task<bool> ImportAsync(ApplicationDbContext context, string filePath)
        {
            //filePath = AppDomain.CurrentDomain.BaseDirectory + '\\' + "serialized.txt";

            ProductsInfo productsInfo;
            try
            {
                string serialized = File.ReadAllText(filePath);
                productsInfo = JsonConvert.DeserializeObject<ProductsInfo>(serialized);
            }
            catch (Exception)
            {
                return false;
            }

            await productsInfo.SaveToDbContextAsync(context);

            return true;
        }
    }
}
