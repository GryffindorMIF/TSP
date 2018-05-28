using System.ComponentModel.DataAnnotations;

namespace EShop.Models.ViewModels
{
    public class ProductInCartViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        [Range(1, 100, ErrorMessage = "Amount of products cannot be less than 1 or greater than 100.")]
        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }
        public string ImageUrl { get; set; }
    }
}