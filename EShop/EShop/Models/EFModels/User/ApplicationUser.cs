using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace EShop.Models.EFModels.User
{
    // Extension of table: Users
    public class ApplicationUser : IdentityUser
    {
        public bool IsSuspended { get; set; }
        public bool IsAdmin { get; set; }

        [ForeignKey("ShoppingCartId")] public int? ShoppingCartId { get; set; }
    }
}