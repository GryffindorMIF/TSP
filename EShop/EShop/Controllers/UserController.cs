using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EShop.Data;
using EShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EShop.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var users = from u in _context.Users
                        join ur in _context.UserRoles on u.Id equals ur.UserId
                        join r in _context.Roles on ur.RoleId equals r.Id
                        select new UserInRoleViewModel
                        {
                            User = u,
                            Role = r.Name
                        };

            return View(users);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Suspend(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        [HttpPost, ActionName("Suspend")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuspendConfirmed(string id)
        {     
            var user = await _userManager.FindByIdAsync(id);
            var isLocked = await _userManager.IsLockedOutAsync(user);

            if (!isLocked)
            {
                // Lockout only works if both properties are set (lockoutEnabled and lockoutEndDate)
                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTime.Today.AddYears(200));// suspend forever

                // Lockout status check without using _userManager (in Razor view pages)
                user.IsSuspended = true;
                await _userManager.UpdateAsync(user);
            }
            
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Restore(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        [HttpPost, ActionName("Restore")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var isLocked = await _userManager.IsLockedOutAsync(user);

            if (isLocked)
            {
                // Lockout only works if both properties are set (lockoutEnabled and lockoutEndDate)
                await _userManager.SetLockoutEnabledAsync(user, false);
                await _userManager.SetLockoutEndDateAsync(user, null);

                // Lockout status check without using _userManager (in Razor view pages)
                user.IsSuspended = false;
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GiveAdminPrivileges(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        [HttpPost, ActionName("GiveAdminPrivileges")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GiveAdminPrivilegesConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            // Will always return single value list (one role per user)
            var roles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRoleAsync(user, roles[0]);
            await _userManager.AddToRoleAsync(user, "Admin");

            // Admin status check without using _userManager (in Razor view pages)
            user.IsAdmin = true;
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoveAdminPrivileges(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
        }

        [HttpPost, ActionName("RemoveAdminPrivileges")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAdminPrivilegesConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            // Will always return single value list (one role per user)
            var roles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRoleAsync(user, roles[0]);
            await _userManager.AddToRoleAsync(user, "Customer");

            // Admin status check without using _userManager (in Razor view pages)
            user.IsAdmin = false;
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }
        private bool ApplicationUserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}