using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    public class AttributeListViewModel
    {
        public ICollection<Attribute> Attributes { get; set; }
        public ICollection<AttributeValue> AttributeValues { get; set; }
    }
}
