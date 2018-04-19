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

            if(user.ShoppingCartId != null)// ADMIN neturi shopping-cart
            { 
                var shoppingCart = await _context.ShoppingCart.FindAsync(user.ShoppingCartId);


                //Removed since with ondelete cascade, this is no longer needed
                /*var ShoppingCartProducts = from scp in _context.ShoppingCartProduct
                                           where scp.ShoppingCart.Id == shoppingCart.Id
                                           select scp;

                foreach (ShoppingCartProduct scp in ShoppingCartProducts)
                {
                    _context.ShoppingCartProduct.Remove(scp);
                }*/

                _context.ShoppingCart.Remove(shoppingCart);

                await _context.SaveChangesAsync();
            }

            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageAccountSuspension(string id, bool suspendAccount)
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

            if(suspendAccount) return View("Suspend", applicationUser);
            else return View("Restore", applicationUser);
        }

        [HttpPost, ActionName("ManageAccountSuspension")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageAccountSuspensionConfirmed(string id, bool suspendAccount)
        {
            var user = await _userManager.FindByIdAsync(id);
            var isLocked = await _userManager.IsLockedOutAsync(user);

            if (isLocked && !suspendAccount)
            {
                // Lockout only works if both properties are set (lockoutEnabled and lockoutEndDate)
                await _userManager.SetLockoutEnabledAsync(user, false);
                await _userManager.SetLockoutEndDateAsync(user, null);

                // To check lockout status without using _userManager (in Razor view pages)
                user.IsSuspended = false;
                await _userManager.UpdateAsync(user);
            }
            else if(!isLocked && suspendAccount)
            {
                // Lockout only works if both properties are set (lockoutEnabled and lockoutEndDate)
                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTime.Today.AddYears(200)); // forever

                // To check lockout status without using _userManager (in Razor view pages)
                user.IsSuspended = true;
                await _userManager.UpdateAsync(user);
                await _userManager.UpdateSecurityStampAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageAdminPrivileges(string id, bool grantPrivileges)
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
            if(grantPrivileges) return View("GiveAdminPrivileges", applicationUser);
            else return View("RemoveAdminPrivileges", applicationUser);
        }


        [HttpPost, ActionName("ManageAdminPrivileges")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageAdminPrivilegesConfirmed(string id, bool grantPrivileges)
        {
            var user = await _userManager.FindByIdAsync(id);

            // Will always return single value list (one role per user)
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRoleAsync(user, roles[0]);

            if(grantPrivileges)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                user.IsAdmin = true;
                await _userManager.UpdateAsync(user);
                await _userManager.UpdateSecurityStampAsync(user);
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Customer");
                // To check admin status without using _userManager (in Razor view pages)
                user.IsAdmin = false;
                await _userManager.UpdateAsync(user);
                await _userManager.UpdateSecurityStampAsync(user);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}