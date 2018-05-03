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
    }
}
