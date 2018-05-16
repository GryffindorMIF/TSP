using EShop.Business.Interfaces;
using EShop.Data;
using EShop.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business.Services
{
    public class DataPortingService : IDataPortingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _appEnvironment;
        private readonly string _productsImagePath, _attributeImagePath, _carouselImagePath;

        public DataPortingService(ApplicationDbContext context, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _appEnvironment = appEnvironment;
            _productsImagePath = Path.Combine(appEnvironment.WebRootPath, "images\\products");
            _attributeImagePath = Path.Combine(appEnvironment.WebRootPath, "images\\attribute-icons");
            _carouselImagePath = Path.Combine(appEnvironment.WebRootPath, "images\\main carousel");
        }

        public async Task<byte[]> ExportProductData()
        {
            byte[] exportData = null;
            await Task.Run(() =>
            {
                exportData = ProductDbPorter.Export(_context, _productsImagePath, _attributeImagePath, _carouselImagePath);
            });

            return exportData;
        }

        public async Task<bool> ImportProductData(IFormFile file)
        {
            bool importSuccessful = false;
            await Task.Run(() =>
            {
                importSuccessful = ProductDbPorter.Import(_context, file, _productsImagePath, _attributeImagePath, _carouselImagePath);
            });
            return importSuccessful;
        }

        public async Task WipeProductData()
        {
            await Task.Run(() =>
            {
                ProductDbPorter.WipeDBProducts(_context);
                ProductDbPorter.WipeImages(_productsImagePath, _attributeImagePath, _carouselImagePath);
            });
        }
    }
}
