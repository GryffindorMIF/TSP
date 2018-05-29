using Microsoft.AspNetCore.Http;

namespace EShop.Models.ViewModels
{
    public class DataImportViewModel
    {
        public IFormFile ImportFile { get; set; }
    }
}