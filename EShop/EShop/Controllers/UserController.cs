using System;
using System.Linq;
using System.Threading.Tasks;
using EShop.Business;
using EShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace EShop.Controllers
{
    [Authorize(Roles = "Admin, SuperAdmin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly int usersPerPage;

        public UserController(UserManager<ApplicationUser> userManager, IUserService userService, IConfiguration configuration)
        {
            _userManager = userManager;
            _userService = userService;

            if (!int.TryParse(configuration["PaginationConfig:UsersPerPage"], out usersPerPage))
            {
                throw new InvalidOperationException("Invalid PaginationConfig:UsersPerPage in appsettings.json. Not an int value.");
            }
        }

        public async Task<IActionResult> Index(int pageNumber = 0)
        {
            //IQueryable<UserInRoleViewModel> users = null;

            // Tuple<pageCount, usersInRoles>
            Tuple<int, IQueryable<UserInRoleViewModel>> tuple;

            if (HttpContext.User.IsInRole("SuperAdmin"))
            {
                //users = await _userService.QueryUsersInRoles(new string[]{ "Customer", "Admin" }, new string[] { _userManager.GetUserId(HttpContext.User) });
                tuple = await _userService.QueryUsersInRolesByPageAsync(pageNumber, usersPerPage);
            }
            else
            {
                //users = await _userService.QueryUsersInRoles(new string[] { "Customer" }, new string[] { _userManager.GetUserId(HttpContext.User) });
                tuple = await _userService.QueryUsersInRolesByPageAsync(pageNumber, usersPerPage);
            }
            ViewBag.CurrentUserRoles = await _userManager.GetRolesAsync(await _userManager.GetUserAsync(HttpContext.User));

            // pagination
            int pageCount = tuple.Item1;
            ViewBag.PageCount = pageCount;

            if (pageNumber + 1 < pageCount) ViewBag.NextPageNumber = pageNumber + 1;
            else ViewBag.NextPageNumber = null;
            if (pageNumber > 0) ViewBag.PreviousPageNumber = pageNumber - 1;
            else ViewBag.PreviousPageNumber = null;

            ViewBag.CurrentPageNumber = pageNumber;

            return View(tuple.Item2);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (await _userService.IsInRoleById(id, "SuperAdmin") == false)
            {
                if ((await _userService.IsInRoleById(id, "Admin") && User.IsInRole("SuperAdmin")) ||
                    (await _userService.IsInRoleById(id, "Customer") && User.IsInRole("Admin")) ||
                    (await _userService.IsInRoleById(id, "Customer") && User.IsInRole("SuperAdmin")))
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var applicationUser = await _userManager.FindByIdAsync(id);

                    if (applicationUser == null)
                    {
                        return NotFound();
                    }

                    return View(applicationUser);
                }
                else return RedirectToAction(nameof(Index));// jei deletina jau nudeletinta useri
            }
            else return new NotFoundResult();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (await _userService.IsInRoleById(id, "SuperAdmin") == false)
            {
                if ((await _userService.IsInRoleById(id, "Admin") && User.IsInRole("SuperAdmin")) ||
                    (await _userService.IsInRoleById(id, "Customer") && User.IsInRole("Admin")) ||
                    (await _userService.IsInRoleById(id, "Customer") && User.IsInRole("SuperAdmin")))
                {
                    var user = await _userManager.FindByIdAsync(id);

                    await _userService.DestroyAllCustomerData(user);

                    await _userManager.DeleteAsync(user);

                    return RedirectToAction(nameof(Index));
                }
                else return new NotFoundResult();
            }
            else return new NotFoundResult();
        }

        public async Task<IActionResult> ManageAccountSuspension(string id, bool suspendAccount)
        {
            if (await _userService.IsInRoleById(id, "SuperAdmin") == false)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var applicationUser = await _userManager.FindByIdAsync(id);
                
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
            if (await _userService.IsInRoleById(id, "SuperAdmin") == false)
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

                await _userManager.UpdateAsync(user);

                return RedirectToAction(nameof(Index));
            }
            else return new NotFoundResult();
        }

        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ManageAdminPrivileges(string id, bool grantPrivileges)
        {
            if (await _userService.IsInRoleById(id, "SuperAdmin") == false)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var applicationUser = await _userManager.FindByIdAsync(id);

                if (applicationUser == null)
                {
                    return NotFound();
                }
                if (grantPrivileges) return View("GiveAdminPrivileges", applicationUser);
                else return View("RemoveAdminPrivileges", applicationUser);
            }
            else return new NotFoundResult();
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost, ActionName("ManageAdminPrivileges")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageAdminPrivilegesConfirmed(string id, bool grantPrivileges)
        {
            if (await _userService.IsInRoleById(id, "SuperAdmin") == false)
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

                    await _userService.DestroyAllCustomerData(user);
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
    }
}