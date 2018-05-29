using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EShop.Business.Interfaces
{
    public enum ImportResult
    {
        Successful,
        Unsuccesful,
        AlreadyRunning
    }

    public interface IDataPortingService
    {
        Task WipeProductDataAsync();
        Task<ImportResult> ImportProductData(IFormFile file);
        Task<byte[]> ExportProductData();
    }
}