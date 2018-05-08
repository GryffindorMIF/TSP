using EShop.Data;
using EShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product> FindProductByIdAsync(int id)
        {
            Product product = null;
            await Task.Run(() =>
            {
                product = _context.Product.FirstOrDefault(p => p.Id == id);
            });
            return product;
        }

        public async Task<Decimal?> GetDiscountPrice(Product product)
        {
            ProductDiscount discount = null;

            await Task.Run(() =>
            {
                discount = (from pd in _context.ProductDiscount
                            where pd.ProductId == product.Id
                            select pd).FirstOrDefault();
            });

            if (discount != null) return discount.DiscountPrice;
            else return null;
        }

        //Retrieve primary image link for every product in passed collection
        public async Task<String[]> GetAllImages(ICollection<Product> products, bool isPrimary = true)
        {
            String[] allImageLinks = new String[products.Count];
            if (products.Count > 0)
            { //To avoid null reference exception
                await Task.Run(() =>
                {
                    var listProducts = products.ToList();
                    for (int i = 0; i < listProducts.Count; i++)
                    {
                        List<ProductImage> image = (from pi in _context.ProductImage
                                                           where pi.IsPrimary == isPrimary
                                                           where pi.Product == listProducts[i]
                                                           select pi).ToList();
                        if (image.Count > 0)
                        {
                            allImageLinks[i] = image[0].ImageUrl;
                        }
                        else
                        {
                            allImageLinks[i] = "product-image-placeholder.jpg";
                        }
                    }
                });
            }
            return allImageLinks;
        }

        public async Task<ICollection<ProductProperty>> GetAllPropertiesByProductIdAsync(int id)
        {
            ICollection<ProductProperty> properties = new List<ProductProperty>();

            await Task.Run(() =>
            {
                properties = _context.ProductProperty.Where(p => p.ProductId == id).ToList();
            });

            return properties;
        }

        public async Task<ProductProperty> FindProductPropertyByIdAsync(int id)
        {
            ProductProperty property = null;
            await Task.Run(() =>
            {
                property = _context.ProductProperty.FirstOrDefault(p => p.Id == id);
            });
            return property;
        }
    }
}
