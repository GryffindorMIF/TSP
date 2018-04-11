using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    public class UserInRoleViewModel
    {
        public ApplicationUser User { get; set; }
        public string Role { get; set; }
    }
}
