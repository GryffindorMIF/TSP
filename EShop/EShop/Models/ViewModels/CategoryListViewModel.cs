using System.Collections.Generic;
using EShop.Models.EFModels.Category;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EShop.Models.ViewModels
{
    public class CategoryListViewModel
    {
        public SelectList Categories { get; set; }
        public int SelectedCategoryId { get; set; }
    }
}
