using EShop.Data;
using EShop.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task<ICollection<Product>> GetAllProducts()
        {
            return await _context.Product.ToListAsync();
        }
        public async Task<Product> FindProductByIdAsync(int id)
        {
            return await _context.Product.FindAsync(id);
        }

        public async Task<Product> FindProductByName(string name)
        {
            return await _context.Product.Where(p => p.Name == name).FirstOrDefaultAsync();
        }

        public Product FindProductById(int id)
        {
            return _context.Product.Find(id);
        }

        public async Task UpdateRowVersionEntry(Product product)
        {
            _context.Entry(product).Property("RowVersion").OriginalValue = product.RowVersion;
            _context.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task CreateProduct(Product product)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProduct(Product product)
        {
            _context.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProduct(int productId)
        {
            _context.Remove(FindProductById(productId));
            await _context.SaveChangesAsync();
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

        public async Task<IList<ProductImage>> GetPrimaryImages(Product product)
        {
            return await (from pi in _context.ProductImage
                          where pi.IsPrimary == true
                          where pi.Product == product
                          select pi).ToListAsync();
        }

        public async Task<IList<ProductImage>> GetSecondaryImages(Product product)
        {
            return await (from pi in _context.ProductImage
                          where pi.IsPrimary == false
                          where pi.Product == product
                          select pi).ToListAsync();
        }

        public async Task<IList<ProductImage>> GetAllProductImages(int productId)
        {
            return await (from oi in _context.ProductImage
                          where oi.Product.Id == productId
                          select oi).ToListAsync();
        }

        public async Task CreateProductImage(ProductImage productImage)
        {
            _context.Add(productImage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductImage(ProductImage productImage)
        {
            _context.Remove(productImage);
            await _context.SaveChangesAsync();
        }

        //Retrieve all properties for given product by id asynchronous
        public async Task<ICollection<ProductProperty>> GetAllPropertiesByProductIdAsync(int id)
        {
            ICollection<ProductProperty> properties = await _context.ProductProperty.Where(p => p.ProductId == id).ToListAsync();
            return properties;
        }

        public async Task<ProductProperty> FindProductPropertyByIdAsync(int id)
        {
            return await _context.ProductProperty.FindAsync(id);
        }

        public async Task CreateProductProperty(ProductProperty productProperty)
        {
            _context.Add(productProperty);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductProperty(int Id)
        {
            _context.Remove(FindProductPropertyByIdAsync(Id));
            await _context.SaveChangesAsync();
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

                    if (filteredProducts[i].Name.ToLower().StartsWith(searchText.ToLower()))
                        remove = false;

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

        public async Task<IList<ProductAd>> GetProductAds()
        {
            return await _context.ProductAd.ToListAsync();
        }

        public async Task<ProductAd> GetProductAdById(int Id)
        {
            return await _context.ProductAd.FindAsync(Id);
        }

        public async Task<IList<ProductAd>> ListPossibleAdImages(int productId)
        {
            return await (from pai in _context.ProductAd
                          where pai.Product.Id == productId
                          select pai).ToListAsync();
        }

        public async Task CreateProductAd(ProductAd productAd)
        {
            _context.Add(productAd);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAd(ProductAd productAd)
        {
            _context.Remove(productAd);
            await _context.SaveChangesAsync();
        }

        public async Task<ProductDiscount> GetDiscountByProductId(int productId)
        {
            ProductDiscount discount = await (from pd in _context.ProductDiscount
                                              where pd.ProductId == productId
                                              select pd).FirstOrDefaultAsync();
            return discount;
        }

        public async Task<ProductDiscount> GetDiscountById(int id)
        {
            return await _context.ProductDiscount.FindAsync(id);
        }

        public async Task<IList<ProductDiscount>> GetAllDiscounts()
        {
            return await _context.ProductDiscount.ToListAsync();
        }

        public async Task CreateDiscount(ProductDiscount productDiscount)
        {
            _context.Add(productDiscount);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDiscount(ProductDiscount discount)
        {
            _context.Remove(discount);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<AttributeValue>> GetAttributeValues(int id)
        {
            List<AttributeValue> values = await (from a in _context.AttributeValue
                                                 join pa in _context.ProductAttributeValue on id equals pa.ProductId
                                                 where a.Id == pa.AttributeValueId
                                                 select a).ToListAsync();
            return values;
        }

        public async Task<IList<AttributeValue>> GetAttributeValuesInCategory(int categoryId)
        {
            return await (from a in _context.AttributeValue
                          join pc in _context.ProductCategory on categoryId equals pc.CategoryId
                          join p in _context.Product on pc.ProductId equals p.Id
                          join pav in _context.ProductAttributeValue on p.Id equals pav.ProductId
                          join av in _context.AttributeValue on pav.AttributeValueId equals av.Id
                          where a.Id == av.Id
                          select a).Distinct().ToListAsync();
        }

        public async Task<Models.Attribute> GetAttributeById(int id)
        {
            Models.Attribute attribute = await _context.Attribute.FindAsync(id);
            return attribute;
        }

        public async Task AddProductToCategory(ProductCategory productCategory)
        {
            _context.Add(productCategory);
            await _context.SaveChangesAsync();
        }
    }
}
