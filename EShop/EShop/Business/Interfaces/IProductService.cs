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
        Task<String[]> GetAllImages(ICollection<Product> products, bool isPrimary = true);
        Task<Product> GetProductById(int id);
    }
}
