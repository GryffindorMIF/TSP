using System.Collections.Generic;
using EShop.Models.EFModels.Category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EShop.Models.ViewModels.Product
{
    public class ProductCategoryViewModel
    {
        public EFModels.Product.Product Product { get; set; }

        public int[] IdsOfSelectedCategories { get; set; }
        //public bool[] IsCategorySelected { get; set; }
        public List<Category> Categories { get; set; }

        public MultiSelectList CategoryMultiSelectList { get; set; }

        public IFormFile PrimaryImage { get; set; }

        public int[] IdsOfSelectedImages { get; set; }
        public List<IFormFile> OtherImages { get; set; }
        public MultiSelectList ImagesToRemoveSelectList { get; set; }

        //public DateTime DiscountStarts { get; set; }
        //public DateTime DiscountEnds { get; set; }
        //public decimal DiscountPrice { get; set; }
    }
}