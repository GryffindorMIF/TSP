using EShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business
{
    public interface IProductService
    {
        Task<Decimal?> GetDiscountPrice(Product product);
        /*For one product and primary image just create List<Product> add it to the list and then write
         * String[] img = await _productService.GetAllImages(products);
         * String ImageLink = img[0];
         * */
        Task<String[]> GetAllImages(ICollection<Product> products, bool isPrimary = true);
        Task<IList<ProductImage>> GetPrimaryImages(Product product);
        Task<IList<ProductImage>> GetSecondaryImages(Product product);

        //Product Images
        Task<IList<ProductImage>> GetAllProductImages(int productId);
        Task CreateProductImage(ProductImage productImage);
        Task DeleteProductImage(ProductImage productImage);

        //Product
        Task<ICollection<Product>> GetAllProducts();
        Task<Product> FindProductByIdAsync(int id);
        Task<Product> FindProductByName(string name);
        Product FindProductById(int id);
        Task UpdateRowVersionEntry(Product product);
        Task CreateProduct(Product product);
        Task UpdateProduct(Product product);
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
