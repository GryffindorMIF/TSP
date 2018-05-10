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
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            IQueryable<UserInRoleViewModel> users = null;

            if (HttpContext.User.IsInRole("SuperAdmin"))
            {
                users = from u in _context.Users
                        join ur in _context.UserRoles on u.Id equals ur.UserId
                        join r in _context.Roles on ur.RoleId equals r.Id
                        where u.Id != _userManager.GetUserId(HttpContext.User)
                        select new UserInRoleViewModel
                        {
                            User = u,
                            Role = r.Name
                        };
            }
            else
            {
                users = from u in _context.Users
                        join ur in _context.UserRoles on u.Id equals ur.UserId
                        join r in _context.Roles on ur.RoleId equals r.Id
                        where u.Id != _userManager.GetUserId(HttpContext.User)
                        where u.IsAdmin == false
                        select new UserInRoleViewModel
                        {
                            User = u,
                            Role = r.Name
                        };
            }
            ViewBag.CurrentUserRoles = await _userManager.GetRolesAsync(await _userManager.GetUserAsync(HttpContext.User));
            return View(users);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (await IsSuperAdmin(id) == false)
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
            else return new NotFoundResult();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (await IsSuperAdmin(id) == false)
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user.ShoppingCartId != null)// ADMIN neturi shopping-cart
                {
                    var shoppingCart = await _context.ShoppingCart.FindAsync(user.ShoppingCartId);

                    _context.ShoppingCart.Remove(shoppingCart);

                    await _context.SaveChangesAsync();
                }

                await _userManager.DeleteAsync(user);

                return RedirectToAction(nameof(Index));
            }
            else return new NotFoundResult();
        }

        public async Task<IActionResult> ManageAccountSuspension(string id, bool suspendAccount)
        {
            if (await IsSuperAdmin(id) == false)
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

                if (suspendAccount) return View("Suspend", applicationUser);
                else return View("Restore", applicationUser);
            }
            else return new NotFoundResult();
        }

        [HttpPost, ActionName("ManageAccountSuspension")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageAccountSuspensionConfirmed(string id, bool suspendAccount)
        {
            if (await IsSuperAdmin(id) == false)
            {
                var user = await _userManager.FindByIdAsync(id);
                var isLocked = await _userManager.IsLockedOutAsync(user);

                if (isLocked && !suspendAccount)
                {
                    user.LockoutEnd = null;
                    // To check lockout status without using _userManager (in Razor view pages)
                    user.IsSuspended = false;
                }
                else if (!isLocked && suspendAccount)
                {
                    user.LockoutEnd = DateTime.Today.AddYears(200); // forever
                                                                    // To check lockout status without using _userManager (in Razor views)
                    user.IsSuspended = true;
                }

                _context.Update(user);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else return new NotFoundResult();
        }

        public async Task<IActionResult> ManageAdminPrivileges(string id, bool grantPrivileges)
        {
            if (await IsSuperAdmin(id) == false)
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
                if (grantPrivileges) return View("GiveAdminPrivileges", applicationUser);
                else return View("RemoveAdminPrivileges", applicationUser);
            }
            else return new NotFoundResult();
        }


        [HttpPost, ActionName("ManageAdminPrivileges")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageAdminPrivilegesConfirmed(string id, bool grantPrivileges)
        {
            if (await IsSuperAdmin(id) == false)
            {
                var user = await _userManager.FindByIdAsync(id);

                // Will always return single value list (one role per user)
                var roles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRoleAsync(user, roles[0]);

                if (grantPrivileges)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    user.IsAdmin = true;
                    await _userManager.UpdateAsync(user);
                    await _userManager.UpdateSecurityStampAsync(user);

                    //destroy customer data
                    var shoppingCart = await _context.ShoppingCart.FindAsync(user.ShoppingCartId);
                    var shoppingCartProducts = await (from scp in _context.ShoppingCartProduct
                                                        where scp.ShoppingCart.Id == shoppingCart.Id
                                                        select scp).ToListAsync();
                    foreach(var scp in shoppingCartProducts)
                    {
                        _context.Remove(scp);
                    }
                    

                    var orders = await (from o in _context.Order
                                        where o.User.Id == user.Id
                                        select o).ToListAsync();

                    foreach (var o in orders)
                    {
                        _context.Remove(o);
                    }

                    var orderReviews = await (from or in _context.OrderReview
                                              where or.User.Id == user.Id
                                              select or).ToListAsync();
                    foreach (var or in orderReviews)
                    {
                        _context.Remove(or);
                    }

                    var adresses = await (from da in _context.DeliveryAddress
                                              where da.User.Id == user.Id
                                              select da).ToListAsync();
                    foreach (var a in adresses)
                    {
                        _context.Remove(a);
                    }

                    _context.Remove(shoppingCart);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    // To check admin status without using _userManager (in Razor views)
                    user.IsAdmin = false;
                    await _userManager.UpdateAsync(user);
                    await _userManager.UpdateSecurityStampAsync(user);
                }

                return RedirectToAction(nameof(Index));
            }
            else return new NotFoundResult();
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private async Task<bool> IsSuperAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("SuperAdmin"))
            {
                return true;
            }
            else return false;
        }
    }
}