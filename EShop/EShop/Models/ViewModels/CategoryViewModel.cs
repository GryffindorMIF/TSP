using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    public class CategoryViewModel
    {
        public Category Category { get; set; }
        public ICollection<CategoryViewModel> SubCategories { get; set; }
    }
}
