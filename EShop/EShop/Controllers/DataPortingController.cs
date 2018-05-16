using System.Threading.Tasks;
using EShop.Business.Interfaces;
using EShop.Data;
using EShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Controllers
{
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class DataPortingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataPortingService _dataPortingService;

        public DataPortingController(ApplicationDbContext context, IDataPortingService dataPortingService)
        {
            _context = context;
            _dataPortingService = dataPortingService;
        }

        // GET: DataPorting
        public async Task<IActionResult> Index()
        {
            if (TempData["Error"] != null)
            {
                ModelState.AddModelError(string.Empty, TempData["Error"].ToString());
            }
            else if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Export()
        {
            byte[] exportData = await _dataPortingService.ExportProductData();

            if (exportData != null)
            {
                return File(exportData, "application/octet-stream","exported_data.xlsx");
            }
            else
            {
                TempData["Error"] = "An error occured while trying to export product data";
                return RedirectToAction("Index", "DataPorting");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(DataImportViewModel model)
        {
            bool importSuccessful = await _dataPortingService.ImportProductData(model.ImportFile);


            if (importSuccessful)
            {
                TempData["Success"] = "Successfully imported product data from file.";
            }
            else
            {
                TempData["Error"] = "An error occured while trying to import product data. Make sure the file is a valid product data Excel file and try again.";
            }
            return RedirectToAction("Index", "DataPorting");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Wipe()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WipeConfirmed()
        {
            await _dataPortingService.WipeProductData();
            TempData["Success"] = "Successfully wiped all product data.";
            return RedirectToAction("Index", "DataPorting");
        }


    }
}