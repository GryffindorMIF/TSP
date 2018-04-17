using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    // For AJAX request (encapsulation)
    public class ProductToCartPostModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
