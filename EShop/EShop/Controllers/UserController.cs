using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EShop.Data;
using EShop.Models;
using EShop.Models.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShop.Controllers
{
    public class UserController : Controller
    {
        private readonly DatabaseContext _context;

        public UserController(DatabaseContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.CustomerData
                .Include(cd => cd.User)
                .ToListAsync());
        }

        [HttpGet]
        public IActionResult Register()
        {

            return View(new CustomerData());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Username, Password")] User user,
            [Bind("FirstName, LastName, Email, PhoneNumber")] CustomerData customerData)
        {
            if (ModelState.IsValid)
            {
                customerData.User = user;

                _context.Add(user);
                _context.Add(customerData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(new CustomerData());
            }
        }

       [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerData = await _context.CustomerData
                .SingleOrDefaultAsync(m => m.Id == id);
            if (customerData == null)
            {
                return NotFound();
            }

            return View(customerData);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customerData = await _context.CustomerData.SingleOrDefaultAsync(m => m.Id == id);
            _context.CustomerData.Remove(customerData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            if(ModelState.IsValid)
            {

            }
            return View(user);
        }
    }
}