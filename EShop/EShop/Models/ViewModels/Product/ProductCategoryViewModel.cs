using Microsoft.AspNetCore.Http;
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

        public IFormFile PrimaryImage { get; set; }

        public int[] IdsOfSelectedImages { get; set; }
        public List<IFormFile> OtherImages { get; set; }
        public MultiSelectList ImagesToRemoveSelectList { get; set; }

        public DateTime DiscountStarts { get; set; }
        public DateTime DiscountEnds { get; set; }
        public decimal DiscountPrice { get; set; }
    }
}
