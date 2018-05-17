using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business.Interfaces
{
    public interface IDataPortingService
    {
        Task WipeProductDataAsync();
        Task<bool> ImportProductData(IFormFile file);
        Task<byte[]> ExportProductData();

    }
}
