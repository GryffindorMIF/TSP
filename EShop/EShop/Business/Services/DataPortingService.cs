using System.IO;
using System.Threading.Tasks;
using EShop.Business.Interfaces;
using EShop.Data;
using EShop.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace EShop.Business.Services
{
    public class DataPortingService : IDataPortingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataPortingTrackerService _portingTracker;
        private readonly string _productsImagePath, _attributeImagePath, _carouselImagePath;

        public DataPortingService(ApplicationDbContext context, IHostingEnvironment appEnvironment,
            IDataPortingTrackerService portingTracker)
        {
            _context = context;
            _portingTracker = portingTracker;
            _productsImagePath = Path.Combine(appEnvironment.WebRootPath, "images\\products");
            _attributeImagePath = Path.Combine(appEnvironment.WebRootPath, "images\\attribute-icons");
            _carouselImagePath = Path.Combine(appEnvironment.WebRootPath, "images\\main carousel");
        }

        public async Task<byte[]> ExportProductData()
        {
            var exportData = await ProductDbPorter.ExportAsync(_context, _productsImagePath, _attributeImagePath,
                _carouselImagePath);
            return exportData;
        }

        public async Task<ImportResult> ImportProductData(IFormFile file)
        {
            if (_portingTracker.IsImportRunning()) return ImportResult.AlreadyRunning;
            _portingTracker.SetImportRunningStatus(true);
            var importSuccessful = await ProductDbPorter.ImportAsync(_context, file, _productsImagePath,
                _attributeImagePath, _carouselImagePath);
            _portingTracker.SetImportRunningStatus(false);
            return importSuccessful ? ImportResult.Successful : ImportResult.Unsuccesful;
        }

        public async Task WipeProductDataAsync()
        {
            await ProductDbPorter.WipeDbProductsAsync(_context);
            //ProductDbPorter.WipeImages(_productsImagePath, _attributeImagePath, _carouselImagePath);
        }
    }
}