using System;
using System.Linq;
using System.Threading.Tasks;
using EShop.Models.EFModels.User;
using EShop.Models.ViewModels;

namespace EShop.Business.Interfaces
{
    public interface IUserService
    {
        Task<int> DestroyAllCustomerData(ApplicationUser user);
        Task<IQueryable<UserInRoleViewModel>> QueryUsersInRoles(string[] roles, string[] excludeUserIds);

        // Tuple<pageCount, usersInRoles>
        Task<Tuple<int, IQueryable<UserInRoleViewModel>>>
            QueryUsersInRolesByPageAsync(int pageNumber, int usersPerPage);

        Task<bool> ApplicationUserExists(string id);
        Task<bool> IsInRoleById(string id, string role);
    }
}