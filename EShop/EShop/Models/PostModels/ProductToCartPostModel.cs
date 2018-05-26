using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    // For AJAX request (encapsulation)
    public class ProductToCartPostModel
    {
        public int ProductId { get; set; }
        [Range(1, 100, ErrorMessage = "Amount of products cannot be less than 1 or greater than 100.")]
        public int Quantity { get; set; }
    }
}
