using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
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
