using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EShop.Models.ViewModels.Product
{
    public class ProductAdViewModel
    {
        public int SelectedProductId { get; set; }
        public SelectList ProductSelectList { get; set; }

        public IFormFile ProductAdImage { get; set; }

        public int[] IdsOfSelectedAdsToRemove { get; set; }
        public MultiSelectList AdsToRemoveSelectList { get; set; }
    }
}