using EShop.Data;
using EShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<int> DestroyAllCustomerData(ApplicationUser user)
        {
            try
            {
                var shoppingCart = await _context.ShoppingCart.FindAsync(user.ShoppingCartId);

                if (shoppingCart != null)
                {
                    var shoppingCartProducts = await (from scp in _context.ShoppingCartProduct
                                                      where scp.ShoppingCart.Id == shoppingCart.Id
                                                      select scp).ToListAsync();
                    foreach (var scp in shoppingCartProducts)
                    {
                        _context.Remove(scp);
                    }
                    _context.Remove(shoppingCart);
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

                var scphs = await (from scph in _context.ShoppingCartProductHistory
                                          join sc in _context.ShoppingCart on scph.ShoppingCartId equals sc.Id
                                          where user.ShoppingCartId == sc.Id
                                          select scph).ToListAsync();

                foreach(var scph in scphs)
                {
                    _context.Remove(scph);
                }

                await _context.SaveChangesAsync();

                return 0;// success
            }
            catch
            {
                return 1;// error
            }
        }

        public async Task<IQueryable<UserInRoleViewModel>> QueryUsersInRoles(string[] roles, string[] excludeUserIds)
        {
            ICollection<ApplicationUser> users = await (from u in _context.Users
                                                  join ur in _context.UserRoles on u.Id equals ur.UserId
                                                  join r in _context.Roles on ur.RoleId equals r.Id
                                                  select u).ToListAsync();

            ICollection<ApplicationUser> usersFilteredByIds = new List<ApplicationUser>();

            foreach (var user in users)
            {
                foreach(var excludeUserId in excludeUserIds)
                {
                    if(!(user.Id == excludeUserId))
                    {
                        usersFilteredByIds.Add(user);
                    }
                }
            }

            ICollection<UserInRoleViewModel> usersInRoles = new List<UserInRoleViewModel>();

            foreach (var user in usersFilteredByIds)
            {
                foreach (var role in roles)
                {
                    if (await _userManager.IsInRoleAsync(user, role))
                    {
                        UserInRoleViewModel userInRole = new UserInRoleViewModel()
                        {
                            User = user,
                            Role = role
                        };
                        usersInRoles.Add(userInRole);
                    }
                }
            }

            return usersInRoles.AsQueryable();
        }

        public async Task<bool> ApplicationUserExists(string id)
        {
            if (await _userManager.FindByIdAsync(id) != null)
                return true;
            else return false;
        }

        public async Task<bool> IsInRoleById(string id, string role)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains(role))
            {
                return true;
            }
            else return false;
        }

        public async Task<Tuple<int,IQueryable<UserInRoleViewModel>>> QueryUsersInRolesByPageAsync(int pageNumber, int usersPerPage)
        {
            ICollection<UserInRoleViewModel> usersInRoles = await (from u in _context.Users
                                                                    join ur in _context.UserRoles on u.Id equals ur.UserId
                                                                    join r in _context.Roles on ur.RoleId equals r.Id
                                                                    select new UserInRoleViewModel()
                                                                    {
                                                                        User = u,
                                                                        Role = r.Name
                                                                    }
                                                                    ).Skip(pageNumber * usersPerPage).Take(usersPerPage).ToListAsync();
            int pageCount = 0;
            int totalUserCount = await (from u in _context.Users
                                        join ur in _context.UserRoles on u.Id equals ur.UserId
                                        join r in _context.Roles on ur.RoleId equals r.Id
                                        select new UserInRoleViewModel()
                                        {
                                            User = u,
                                            Role = r.Name
                                        }).CountAsync();

            pageCount = totalUserCount / usersPerPage;

            if (totalUserCount % usersPerPage != 0)
            {
                pageCount++;
            }

            return new Tuple<int, IQueryable<UserInRoleViewModel>>(pageCount, usersInRoles.AsQueryable());
        }
    }
}
