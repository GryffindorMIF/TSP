using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EShop.Models
{
    // Extension of table: Users
    public class ApplicationUser : IdentityUser
    {
        public bool IsSuspended { get; set; }
        public bool IsAdmin { get; set; }

        public int? ShoppingCartId { get; set; }
        [ForeignKey("ShoppingCartId")]
        public virtual ShoppingCart ShoppingCart { get; set; }
    }
}
