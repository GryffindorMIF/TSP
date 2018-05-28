using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShop.Business.Interfaces;
using EShop.Data;
using EShop.Models.EFModels.Attribute;
using EShop.Models.EFModels.Product;
using Microsoft.EntityFrameworkCore;
using Attribute = EShop.Models.EFModels.Attribute.Attribute;

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

        public Product FindProductById(int id)
        {
            return _context.Product.Find(id);
        }

        public async Task<Product> FindProductByName(string name)
        {
            return await _context.Product.Where(p => p.Name == name).FirstOrDefaultAsync();
        }

        public async Task UpdateRowVersionEntry(Product product)
        {
            _context.Entry(product).Property("RowVersion").OriginalValue = product.RowVersion;
            _context.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CreateProduct(Product product)
        {
            try
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public async Task<int> UpdateProduct(Product product)
        {
            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
                return 0;
            }
            catch (Exception)
            {
                return 1;
            }
        }

        public async Task DeleteProduct(int productId)
        {
            _context.Remove(await FindProductByIdAsync(productId));
            await _context.SaveChangesAsync();
        }

        public async Task<decimal?> GetDiscountPrice(Product product)
        {
            ProductDiscount discount = null;

            await Task.Run(() =>
            {
                discount = (from pd in _context.ProductDiscount
                    where pd.ProductId == product.Id
                    select pd).FirstOrDefault();
            });

            if (discount != null) return discount.DiscountPrice;
            return null;
        }

        //Retrieve primary image link for every product in passed collection
        public async Task<string[]> GetProductsImagesLinks(ICollection<Product> products, bool isPrimary = true)
        {
            //If it is primary images request, then links count should be products amount, otherwise - unknown yet 
            var allImageLinks = isPrimary ? new string[products.Count] : new string[0];

            if (products.Count > 0) //To avoid null reference exception
                await Task.Run(() =>
                {
                    var listProducts = products.ToList();
                    for (var i = 0; i < listProducts.Count; i++)
                    {
                        var index = i;
                        var images = (from pi in _context.ProductImage
                            where pi.IsPrimary == isPrimary
                            where pi.Product.Id == listProducts[index].Id
                            select pi).ToList();
                        if (isPrimary) //If request was made to get primary image(-s)
                        {
                            if (images.Count > 0)
                                allImageLinks[index] = images[0].ImageUrl;
                            else
                                allImageLinks[index] = "product-image-placeholder.jpg";
                        }
                        else if (images.Count > 0
                        ) //In case request was made to get all secondary images links
                        {
                            allImageLinks = new string[images.Count];
                            for (var j = 0; j < images.Count; j++) allImageLinks[j] = images[j].ImageUrl;
                        }
                    }
                });
            return allImageLinks;
        }

        public async Task<IList<ProductImage>> GetProductImages(int productId, bool isPrimary = true)
        {
            var images = new List<ProductImage>();
            var result = 0;

            await Task.Run(() =>
            {
                try
                {
                    images = (from pi in _context.ProductImage
                        where pi.IsPrimary == isPrimary
                        where pi.Product.Id == productId
                        select pi).ToList();
                }
                catch (Exception)
                {
                    result = -1;
                }
            });

            return result == 0 ? images : null;
        }

        /* public async Task<IList<ProductImage>> GetPrimaryImages(Product product)
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
         }*/

        public async Task<IList<ProductImage>> GetAllProductImages(int productId)
        {
            return await (from oi in _context.ProductImage
                where oi.Product.Id == productId
                select oi).ToListAsync();
        }

        public async Task AddProductImage(ProductImage productImage)
        {
            await _context.AddAsync(productImage);
            //await _context.SaveChangesAsync(); ANDRIUS DEBILAS SITO LINE NEREIKIA
        }

        public async Task DeleteProductImage(ProductImage productImage)
        {
            await Task.Run(() => { _context.Remove(productImage); });
            await _context.SaveChangesAsync();
        }

        //Retrieve all properties for given product by id asynchronous
        public async Task<ICollection<ProductProperty>> GetAllPropertiesByProductIdAsync(int id)
        {
            ICollection<ProductProperty> properties =
                await _context.ProductProperty.Where(p => p.ProductId == id).ToListAsync();
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

        public async Task DeleteProductProperty(int id)
        {
            _context.Remove(await FindProductPropertyByIdAsync(id));
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<Product>> SearchForProducts(string searchText)
        {
            List<Product> filteredProducts = null;

            await Task.Run(() =>
            {
                //filteredProducts = _context.Product.Where(p => p.Name.StartsWith(searchText)).ToList();
                filteredProducts = _context.Product.ToList();
                for (var i = filteredProducts.Count - 1; i > -1; i--)
                {
                    var remove = true;

                    if (filteredProducts[i].Name.ToLower().StartsWith(searchText.ToLower()))
                        remove = false;

                    var nameArray = filteredProducts[i].Name.Split(' ');
                    foreach (var part in nameArray)
                        if (part.ToLower().StartsWith(searchText.ToLower()))
                            remove = false;
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

        public async Task<ProductAd> GetProductAdById(int id)
        {
            return await _context.ProductAd.FindAsync(id);
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
            var discount = await (from pd in _context.ProductDiscount
                where pd.ProductId == productId
                select pd).FirstOrDefaultAsync();
            return discount;
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
            var values = await (from a in _context.AttributeValue
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

        public async Task<Attribute> GetAttributeById(int id)
        {
            var attribute = await _context.Attribute.FindAsync(id);
            return attribute;
        }

        public async Task AddProductToCategory(ProductCategory productCategory)
        {
            _context.Add(productCategory);
            await _context.SaveChangesAsync();
        }

        public async Task<ProductDiscount> GetDiscountById(int id)
        {
            return await _context.ProductDiscount.FindAsync(id);
        }
    }
}