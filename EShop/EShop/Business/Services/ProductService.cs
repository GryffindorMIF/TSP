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
            //If it is primary images request, then links count should be products amount, otherwise - unknown yet 
            String[] allImageLinks = isPrimary ? new String[products.Count] : new String[0];

            if (isPrimary)
                allImageLinks = new String[products.Count];
            else allImageLinks = new String[0];

            if (products.Count > 0) //To avoid null reference exception
            {
                await Task.Run(() =>
                {
                    var listProducts = products.ToList();
                    for (int i = 0; i < listProducts.Count; i++)
                    {
                        List<ProductImage> images = (from pi in _context.ProductImage
                                                     where pi.IsPrimary == isPrimary
                                                     where pi.Product == listProducts[i]
                                                     select pi).ToList();
                        if (isPrimary) //If request was made to get primary image(-s)
                        {
                            if (images.Count > 0)
                            {
                                allImageLinks[i] = images[0].ImageUrl;
                            }
                            else
                            {
                                allImageLinks[i] = "product-image-placeholder.jpg";
                            }
                        }
                        else if (!isPrimary && images.Count > 0) //In case request was made to get all secondary images links
                        {
                            allImageLinks = new String[images.Count];
                            for (int j = 0; j < images.Count; j++)
                            {
                                allImageLinks[j] = images[j].ImageUrl;
                            }
                        }
                    }
                });
            }
            return allImageLinks;
        }

        //Retrieve all properties for given product by id asynchronous
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

        public async Task<ICollection<Product>> SearchForProducts(string searchText)
        {
            List<Product> filteredProducts = null;

            await Task.Run(() =>
            {
                //filteredProducts = _context.Product.Where(p => p.Name.StartsWith(searchText)).ToList();
                filteredProducts = _context.Product.ToList();
                for (int i = filteredProducts.Count - 1; i > -1; i--)
                {
                    bool remove = true;
                    string[] nameArray = filteredProducts[i].Name.Split(' ');
                    foreach (string part in nameArray)
                    {
                        if (part.ToLower().StartsWith(searchText.ToLower()))
                            remove = false;
                    }
                    if (remove)
                        filteredProducts.RemoveAt(i);
                }
            });
            return filteredProducts;
        }
    }
}
