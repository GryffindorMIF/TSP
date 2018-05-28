using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    public class ProductInCartViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Decimal Price { get; set; }
        [Range(1, 100, ErrorMessage = "Amount of products cannot be less than 1 or greater than 100.")]
        public int Quantity { get; set; }
        public Decimal TotalPrice { get; set; }
        public string ImageUrl { get; set; }
    }
}
