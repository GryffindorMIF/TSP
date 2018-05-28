using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    public class ManageAttributesViewModel
    {
        public int SelectedAttributeId { get; set; }
        public SelectList AttributeSelectList { get; set; }

        public MultiSelectList AttributeyMultiSelectList { get; set; }
        public int[] IdsOfSelectedAttributesToRemove { get; set; }

        public MultiSelectList AttributeValuesMultiSelectList { get; set; }
        public int[] IdsOfSelectedAttributeValues { get; set; }

        public MultiSelectList ProductMultiSelectList { get; set; }
        public int[] IdsOfSelectedProducts { get; set; }

        public MultiSelectList ProductCategoryMultiSelectList { get; set; }
        public int[] IdsOfSelectedProductCategories { get; set; }

        public MultiSelectList LinksMultiList { get; set; }
        public int[] IdsOfSelectedLinks { get; set; }

    }
}
