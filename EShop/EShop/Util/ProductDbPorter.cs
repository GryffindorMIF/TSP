﻿using EShop.Data;
using EShop.Models;
using FastMember;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EShop.Util
{
    public static class ProductDbPorter
    {
        private class ProductsInfo
        {
            public class WrappedProductsInfo
            {
                public List<Tuple<int, Models.Attribute>> Attributes { get; set; }
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

                public WrappedProductsInfo(ProductsInfo productsInfo)
                {
                    Load(productsInfo);
                }

                private void Load(ProductsInfo productsInfo)
                {
                    Attributes = productsInfo.Attributes.Select((a) => { return new Tuple<int, Models.Attribute>(a.Id, a); }).ToList();
                    AttributeValues = productsInfo.AttributeValues.Select((av) => { return new Tuple<int, AttributeValue>(av.Id, av); }).ToList();
                    Categories = productsInfo.Categories.Select((c) => { return new Tuple<int, Category>(c.Id, c); }).ToList();
                    CategoryCategories = productsInfo.CategoryCategories.Select((cc) => { return new Tuple<int, CategoryCategory>(cc.Id, cc); }).ToList();
                    Products = productsInfo.Products.Select((p) => { return new Tuple<int, Product>(p.Id, p); }).ToList();
                    ProductAds = productsInfo.ProductAds.Select((pa) => { return new Tuple<int, ProductAd>(pa.Id, pa); }).ToList();
                    ProductAttributeValues = productsInfo.ProductAttributeValues.Select((pav) => { return new Tuple<int, ProductAttributeValue>(pav.Id, pav); }).ToList();
                    ProductCategories = productsInfo.ProductCategories.Select((pc) => { return new Tuple<int, ProductCategory>(pc.Id, pc); }).ToList();
                    ProductDiscounts = productsInfo.ProductDiscounts.Select((pd) => { return new Tuple<int, ProductDiscount>(pd.Id, pd); }).ToList();
                    ProductImages = productsInfo.ProductImages.Select((pi) => { return new Tuple<int, ProductImage>(pi.Id, pi); }).ToList();
                    ProductProperties = productsInfo.ProductProperties.Select((pp) => { return new Tuple<int, ProductProperty>(pp.Id, pp); }).ToList();
                }
            }



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
            }

            public void SaveToDbContext(ApplicationDbContext context)
            {
                ProductsInfo currentDbState = new ProductsInfo();
                currentDbState.LoadFromDbContext(context);

                WrappedProductsInfo wrappedProductsInfo = new WrappedProductsInfo(this);

                //LEFT TO ADD: Product, ProductDiscount, ProductAd, ProductImage, ProductAttributeValue, AttributeValue, Attribute, ProductProperty, ProductCategory, Category, CategoryCategory
                //ADDING IN THIS STEP: Product, Attribute, Categories

                //Add missing products
                foreach (var importProduct in Products)
                {
                    Product dbProduct = currentDbState.Products.FirstOrDefault((p) => { return p.Equals(importProduct); });
                    if (dbProduct == null)
                    {
                        importProduct.Id = 0;
                        context.Product.Add(importProduct);
                    }
                }

                //Add missing attributes
                foreach (var importAttribute in Attributes)
                {
                    Models.Attribute dbAttribute = currentDbState.Attributes.FirstOrDefault((a) => { return a.Equals(importAttribute); });
                    if (dbAttribute == null)
                    {
                        importAttribute.Id = 0;
                        context.Attribute.Add(importAttribute);
                    }
                }

                //Add missing categories
                foreach (var importCategory in Categories)
                {
                    Category dbCategory = currentDbState.Categories.FirstOrDefault((c) => { return c.Equals(importCategory); });
                    if (dbCategory == null)
                    {
                        importCategory.Id = 0;
                        context.Category.Add(importCategory);
                    }
                }
                context.SaveChanges();

                //Update every entity's ProductId value
                foreach (var updatedProduct in wrappedProductsInfo.Products)
                {
                    ProductDiscounts.Where((x) => { return x.ProductId == updatedProduct.Item1; }).All((x) => { x.ProductId = updatedProduct.Item2.Id; return true; });
                    ProductAds.Where((x) => { return x.ProductId == updatedProduct.Item1; }).All((x) => { x.ProductId = updatedProduct.Item2.Id; return true; });
                    ProductImages.Where((x) => { return x.ProductId == updatedProduct.Item1; }).All((x) => { x.ProductId = updatedProduct.Item2.Id; return true; });
                    ProductAttributeValues.Where((x) => { return x.ProductId == updatedProduct.Item1; }).All((x) => { x.ProductId = updatedProduct.Item2.Id; return true; });
                    ProductProperties.Where((x) => { return x.ProductId == updatedProduct.Item1; }).All((x) => { x.ProductId = updatedProduct.Item2.Id; return true; });
                    ProductCategories.Where((x) => { return x.ProductId == updatedProduct.Item1; }).All((x) => { x.ProductId = updatedProduct.Item2.Id; return true; });
                }

                //Update every entity's AttributeId values
                foreach (var updatedAttribute in wrappedProductsInfo.Attributes)
                {
                    AttributeValues.Where((x) => { return x.AttributeId == updatedAttribute.Item1; }).All((x) => { x.AttributeId = updatedAttribute.Item2.Id; return true; });
                }

                //Update every entity's CategoryId and ParentCategoryId values
                foreach (var updatedCategory in wrappedProductsInfo.Categories)
                {
                    ProductCategories.Where((x) => { return x.CategoryId == updatedCategory.Item1; }).All((x) => { x.CategoryId = updatedCategory.Item2.Id; return true; });
                    CategoryCategories.Where((x) => { return x.CategoryId == updatedCategory.Item1; }).All((x) => { x.CategoryId = updatedCategory.Item2.Id; return true; });
                    CategoryCategories.Where((x) => { return x.ParentCategoryId == updatedCategory.Item1; }).All((x) => { x.ParentCategoryId = updatedCategory.Item2.Id; return true; });
                }

                //LEFT TO ADD: ProductDiscount, ProductAd, ProductImage, ProductAttributeValue, AttributeValue, ProductProperty, ProductCategory, CategoryCategory
                //ADDING IN THIS STEP: ProductDiscount, ProductAd, ProductImage, AttributeValue, ProductProperty, CategoryCategory, ProductCategory

                //Add missing product discounts
                foreach (var importProductDiscount in ProductDiscounts)
                {
                    ProductDiscount dbProductDiscount = currentDbState.ProductDiscounts.FirstOrDefault((pd) => { return pd.Equals(importProductDiscount); });
                    if (dbProductDiscount == null)
                    {
                        importProductDiscount.Id = 0;
                        context.ProductDiscount.Add(importProductDiscount);
                    }
                }
                context.SaveChanges();//testavimui

                //Add missing product ads
                foreach (var importProductAd in ProductAds)
                {
                    ProductAd dbProductAd = currentDbState.ProductAds.FirstOrDefault((pa) => { return pa.Equals(importProductAd); });
                    if (dbProductAd == null)
                    {
                        importProductAd.Id = 0;
                        context.ProductAd.Add(importProductAd);
                    }
                }

                //Add missing product images
                foreach (var importProductImage in ProductImages)
                {
                    ProductImage dbProductImage = currentDbState.ProductImages.FirstOrDefault((pi) => { return pi.Equals(importProductImage); });
                    if (dbProductImage == null)
                    {
                        importProductImage.Id = 0;
                        context.ProductImage.Add(importProductImage);
                    }
                }

                //Add missing product properties
                foreach (var importProductProperty in ProductProperties)
                {
                    ProductProperty dbProductProperty = currentDbState.ProductProperties.FirstOrDefault((pp) => { return pp.Equals(importProductProperty); });
                    if (dbProductProperty == null)
                    {
                        importProductProperty.Id = 0;
                        context.ProductProperty.Add(importProductProperty);
                    }
                }

                //Add missing attribute values
                foreach (var importAttributeValue in AttributeValues)
                {
                    AttributeValue dbAttributeValue = currentDbState.AttributeValues.FirstOrDefault((av) => { return av.Equals(importAttributeValue); });
                    if (dbAttributeValue == null)
                    {
                        importAttributeValue.Id = 0;
                        context.AttributeValue.Add(importAttributeValue);
                    }
                }

                //Add missing category categories
                foreach (var importCategoryCategory in CategoryCategories)
                {
                    CategoryCategory dbCategoryCategory = currentDbState.CategoryCategories.FirstOrDefault((cc) => { return cc.Equals(importCategoryCategory); });
                    if (dbCategoryCategory == null)
                    {
                        importCategoryCategory.Id = 0;
                        context.CategoryCategory.Add(importCategoryCategory);
                    }
                }

                //Add missing product categories
                foreach (var importProductCategory in ProductCategories)
                {
                    ProductCategory dbProductCategory = currentDbState.ProductCategories.FirstOrDefault((pc) => { return pc.Equals(importProductCategory); });
                    if (dbProductCategory == null)
                    {
                        importProductCategory.Id = 0;
                        context.ProductCategory.Add(importProductCategory);
                    }
                }
                context.SaveChanges();

                //Update every entity's AttributeValueId
                foreach (var updatedAttributeValue in wrappedProductsInfo.AttributeValues)
                {
                    ProductAttributeValues.Where((x) => { return x.AttributeValueId == updatedAttributeValue.Item1; }).All((x) => { x.AttributeValueId = updatedAttributeValue.Item2.Id; return true; });
                }

                //LEFT TO ADD: ProductAttributeValue
                //ADDING IN THIS STEP: ProductAttributeValue

                //Add missing product attribute values
                foreach (var importProductAttributeValue in ProductAttributeValues)
                {
                    ProductAttributeValue dbProductAttributeValue = currentDbState.ProductAttributeValues.FirstOrDefault((pav) => { return pav.Equals(importProductAttributeValue); });
                    if (dbProductAttributeValue == null)
                    {
                        importProductAttributeValue.Id = 0;
                        context.ProductAttributeValue.Add(importProductAttributeValue);
                    }
                }


                context.SaveChanges();
            }
        }

        public static bool Export(ApplicationDbContext context, string filePath)
        {
            filePath = AppDomain.CurrentDomain.BaseDirectory + '\\' + "serialized.txt";

            ProductsInfo productsInfo = new ProductsInfo();
            productsInfo.LoadFromDbContext(context);

            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    package.ProductsInfoToSheets(productsInfo);
                    byte[] data = package.GetAsByteArray();

                    string path = AppDomain.CurrentDomain.BaseDirectory + '\\' + "serialized.xlsx";
                    File.WriteAllBytes(path, data);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool Import(ApplicationDbContext context, string filePath)
        {
            filePath = AppDomain.CurrentDomain.BaseDirectory + '\\' + "serialized.xlsx";

            ProductsInfo productsInfo;
            try
            {
                using (var package = new ExcelPackage())
                {
                    using (var stream = File.OpenRead(filePath))
                    {
                        package.Load(stream);
                    }
                    productsInfo = package.SheetsToProductsInfo();
                }

            }
            catch (Exception)
            {
                return false;
            }
            
            productsInfo.SaveToDbContext(context);

            return true;
        }

        public static void WipeDBProducts(ApplicationDbContext context)
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
            DataTable table = new DataTable();
            using (var reader = ObjectReader.Create(data))
            {
                table.Load(reader);
            }

            // Stripping redundant columns (commented out line that would include RowVersion in the serialization)
            Stack<DataColumn> columnsToRemove = new Stack<DataColumn>();
            foreach(DataColumn column in table.Columns)
            {
                MethodInfo methodInfo = column.DataType.GetMethod("ToString", new Type[0]);
                if ( (methodInfo == null || methodInfo.DeclaringType == typeof(object)) /*&& column.DataType != typeof(byte[])*/)
                {
                    columnsToRemove.Push(column);
                }
            }
            while(columnsToRemove.Count > 0)
            {
                var column = columnsToRemove.Pop();
                table.Columns.Remove(column);
            }


            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);
            worksheet.Cells["A1"].LoadFromDataTable(table, true);

            // Fix for date time vars
            int colNumber = 1;
            foreach (DataColumn column in table.Columns)
            {
                if (column.DataType == typeof(DateTime))
                {
                    worksheet.Column(colNumber).Style.Numberformat.Format = "yyyy-MM-dd hh:mm:ss";
                }
                colNumber++;
            }
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }

        #endregion

        #region Import methods

        private static ProductsInfo SheetsToProductsInfo(this ExcelPackage package)
        {
            ProductsInfo info = new ProductsInfo
            {
                Attributes = package.Workbook.Worksheets["Attributes"].GetListFromWorksheet<Models.Attribute>(),
                AttributeValues = package.Workbook.Worksheets["AttributeValues"].GetListFromWorksheet<AttributeValue>(),
                Categories = package.Workbook.Worksheets["Categories"].GetListFromWorksheet<Category>(),
                CategoryCategories = package.Workbook.Worksheets["CategoryCategories"].GetListFromWorksheet<CategoryCategory>(),
                Products = package.Workbook.Worksheets["Products"].GetListFromWorksheet<Product>(),
                ProductAds = package.Workbook.Worksheets["ProductAds"].GetListFromWorksheet<ProductAd>(),
                ProductAttributeValues = package.Workbook.Worksheets["ProductAttributeValues"].GetListFromWorksheet<ProductAttributeValue>(),
                ProductCategories = package.Workbook.Worksheets["ProductCategories"].GetListFromWorksheet<ProductCategory>(),
                ProductDiscounts = package.Workbook.Worksheets["ProductDiscounts"].GetListFromWorksheet<ProductDiscount>(),
                ProductImages = package.Workbook.Worksheets["ProductImages"].GetListFromWorksheet<ProductImage>(),
                ProductProperties = package.Workbook.Worksheets["ProductProperties"].GetListFromWorksheet<ProductProperty>()
            };
            return info;
        }

        private static List<T> GetListFromWorksheet<T>(this ExcelWorksheet worksheet) where T : class, new()
        {
            DataTable table = new DataTable();
            foreach (var firstRowCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
            {
                table.Columns.Add(firstRowCell.Text);
            }

            for (int rowNum = 2; rowNum <= worksheet.Dimension.End.Row; rowNum++)
            {
                var worksheetRow = worksheet.Cells[rowNum, 1, rowNum, worksheet.Dimension.End.Column];
                DataRow row = table.Rows.Add();
                foreach (var cell in worksheetRow)
                {
                    row[cell.Start.Column - 1] = cell.Text;
                }
            }

            return table.ToList<T>();
        }


        private static List<T> ToList<T>(this DataTable table) where T : class, new()
        {
            List<T> list = new List<T>();

            foreach (DataRow row in table.Rows)
            {
                T entity = new T();

                foreach (var prop in entity.GetType().GetProperties())
                {
                    try
                    {
                        PropertyInfo propertyInfo = entity.GetType().GetProperty(prop.Name);
                        propertyInfo.SetValue(entity, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        
                    }
                    catch (Exception)
                    {
                        //Hacky bypass non string and int bugs
                        PropertyInfo propertyInfo = entity.GetType().GetProperty(prop.Name);
                        try { propertyInfo.SetValue(entity, int.Parse(row[prop.Name].ToString()), null);} catch {}
                        continue;
                    }
                }

                list.Add(entity);
            }
            return list;
        }

    #endregion

    class CustomResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty prop = base.CreateProperty(member, memberSerialization);
                var propInfo = member as PropertyInfo;
                if (propInfo != null)
                {
                    if (propInfo.GetMethod.IsVirtual && !propInfo.GetMethod.IsFinal)
                    {
                        prop.ShouldSerialize = obj => false;
                    }
                }
                return prop;
            }
        }
    }
}
