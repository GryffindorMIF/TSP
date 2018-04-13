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

        [ForeignKey("ShoppingCartId")]
        public int? ShoppingCartId { get; set; }
        //public virtual ShoppingCart ShoppingCart { get; set; } --Changed by Denis
    }
}
