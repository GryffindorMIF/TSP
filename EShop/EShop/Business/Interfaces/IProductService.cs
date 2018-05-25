﻿using EShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business
{
    public interface IProductService
    {
        Task<Decimal?> GetDiscountPrice(Product product);

        //Product Images
        //For many products (used in Home/Index)
        Task<String[]> GetProductsImagesLinks(ICollection<Product> products, bool isPrimary = true);
        //For one product (used in Home/ProductPage)
        Task<IList<ProductImage>> GetProductImages(int productId, bool isPrimary = true);
        Task<IList<ProductImage>> GetAllProductImages(int productId);
        Task AddProductImage(ProductImage productImage);
        Task DeleteProductImage(ProductImage productImage);

        //Product
        Task<ICollection<Product>> GetAllProducts();
        Task<Product> FindProductByIdAsync(int id);
        Product FindProductById(int id); //For Rokas space stuff
        Task<Product> FindProductByName(string name);
        Task UpdateRowVersionEntry(Product product);
        Task<int> CreateProduct(Product product);
        Task<int> UpdateProduct(Product product);
        Task DeleteProduct(int productId);

        //Product Properties
        Task<ProductProperty> FindProductPropertyByIdAsync(int id);
        Task<ICollection<ProductProperty>> GetAllPropertiesByProductIdAsync(int id);
        Task CreateProductProperty(ProductProperty productProperty);
        Task DeleteProductProperty(int Id);

        //Product Ads
        Task CreateProductAd(ProductAd productAd);
        Task DeleteProductAd(ProductAd productAd);
        Task<ProductAd> GetProductAdById(int Id);
        Task<IList<ProductAd>> GetProductAds();
        Task<IList<ProductAd>> ListPossibleAdImages(int productId);

        //Product Discounts
        Task<ProductDiscount> GetDiscountByProductId(int productId);
        Task<IList<ProductDiscount>> GetAllDiscounts();
        Task CreateDiscount(ProductDiscount productDiscount);
        Task DeleteDiscount(ProductDiscount discount);

        //Product Attributes
        Task<Models.Attribute> GetAttributeById(int id);
        Task<IList<AttributeValue>> GetAttributeValues(int id);
        Task<IList<AttributeValue>> GetAttributeValuesInCategory(int categoryId);

        Task AddProductToCategory(ProductCategory productCategory);

        //Returning products that satisfy search algorithm by given text 
        Task<ICollection<Product>> SearchForProducts(string searchText);
    }
}
