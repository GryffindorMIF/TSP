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
            public List<Product> Products { get; set; }
            public List<ProductAd> ProductAds { get; set; }
            public List<ProductAttributeValue> ProductAttributeValues { get; set; }
            public List<ProductDiscount> ProductDiscounts { get; set; }
            public List<ProductImage> ProductImages { get; set; }
            public List<ProductProperty> ProductProperties { get; set; }
            public List<Category> Categories { get; set; }
            public List<CategoryCategory> CategoryCategories { get; set; }
            public List<ProductCategory> ProductCategories { get; set; }
            public List<Models.Attribute> Attributes { get; set; }
            public List<AttributeValue> AttributeValues { get; set; }

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

            public void SaveToDbContext(ApplicationDbContext context)
            {

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

        public static bool Import(ApplicationDbContext context, string filePath)
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

            productsInfo.SaveToDbContext(context);

            return true;
        }
    }
}
