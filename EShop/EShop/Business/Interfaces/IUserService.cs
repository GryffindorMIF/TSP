﻿using EShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business
{
    public interface IUserService
    {
        Task<int> DestroyAllCustomerData(ApplicationUser user);
        Task<IQueryable<UserInRoleViewModel>> QueryUsersInRoles(string[] roles, string[] excludeUserIds);

        // Tuple<pageCount, usersInRoles>
        Task<Tuple<int, IQueryable<UserInRoleViewModel>>> QueryUsersInRolesByPageAsync(int pageNumber, int usersPerPage);

        Task<bool> ApplicationUserExists(string id);
        Task<bool> IsInRoleById(string id, string role);
    }
}
