using System.Threading.Tasks;
using EShop.Business.Interfaces;
using EShop.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers
{
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class DataPortingController : Controller
    {
        private readonly IDataPortingService _dataPortingService;

        public DataPortingController(IDataPortingService dataPortingService)
        {
            _dataPortingService = dataPortingService;
        }

        // GET: DataPorting
        public IActionResult Index()
        {
            if (TempData["Error"] != null)
                ModelState.AddModelError(string.Empty, TempData["Error"].ToString());
            else if (TempData["Success"] != null) ViewBag.Success = TempData["Success"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Export()
        {
            var exportData = await _dataPortingService.ExportProductData();

            if (exportData != null) return File(exportData, "application/octet-stream", "exported_data.xlsx");

            TempData["Error"] = "An error occured while trying to export product data";
            return RedirectToAction("Index", "DataPorting");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(DataImportViewModel model)
        {
            var importResult = await _dataPortingService.ImportProductData(model.ImportFile);


            if (importResult == ImportResult.Successful)
            {
                TempData["Success"] = "Successfully imported product data from file.";
            }
            else if (importResult == ImportResult.AlreadyRunning)
            {
                ViewBag.AlreadyRunning =
                    "An import operation is already running. Please wait until it finishes and try again.";
                return View("Index");
            }
            else
            {
                TempData["Error"] =
                    "An error occured while trying to import product data. Make sure the file is a valid product data Excel file and try again.";
            }

            return RedirectToAction("Index", "DataPorting");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Wipe()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WipeConfirmed()
        {
            await _dataPortingService.WipeProductDataAsync();
            TempData["Success"] = "Successfully wiped all product data.";
            return RedirectToAction("Index", "DataPorting");
        }
    }
}