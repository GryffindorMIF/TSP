using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    public class ProductCategoryViewModel
    {
        public Product Product { get; set; }

        public int[] IdsOfSelectedCategories { get; set; }
        public bool[] IsCategorySelected { get; set; }
        public List<Category> Categories { get; set; }

        public MultiSelectList CategoryMultiSelectList { get; set; }
        
    }
}
