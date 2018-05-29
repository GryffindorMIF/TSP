using System.Collections.Generic;
using EShop.Models.EFModels.Category;

namespace EShop.Models.ViewModels
{
    public class CategoryViewModel
    {
        public Category Category { get; set; }
        public ICollection<CategoryViewModel> SubCategories { get; set; }
    }
}