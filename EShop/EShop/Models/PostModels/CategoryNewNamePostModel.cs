using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    public class CategoryNewNamePostModel
    {
        public int CategoryId { get; set; }
        public string NewName { get; set; }
        public string NewDescription { get; set; }
    }
}
