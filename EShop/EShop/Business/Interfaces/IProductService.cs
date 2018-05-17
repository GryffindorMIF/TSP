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
        ICollection<Product> GetAllProducts();
        Task<Product> FindProductByIdAsync(int id);
        Product FindProductById(int id);
        Task<ProductProperty> FindProductPropertyByIdAsync(int id);
        Task<ICollection<ProductProperty>> GetAllPropertiesByProductIdAsync(int id);

        //Returning products that satisfy search algorithm by given text 
        Task<ICollection<Product>> SearchForProducts(string searchText);
    }
}
