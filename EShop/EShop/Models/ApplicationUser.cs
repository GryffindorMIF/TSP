using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    }
}
