using System.Collections.Generic;
using EShop.Models.EFModels.Attribute;

namespace EShop.Models.ViewModels.Home
{
    public class AttributeListViewModel
    {
        public ICollection<Attribute> Attributes { get; set; }
        public ICollection<AttributeValue> AttributeValues { get; set; }
    }
}