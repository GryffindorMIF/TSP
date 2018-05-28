using EShop.Models.EFModels.User;

namespace EShop.Models.ViewModels
{
    public class UserInRoleViewModel
    {
        public ApplicationUser User { get; set; }
        public string Role { get; set; }
    }
}