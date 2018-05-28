using System.ComponentModel.DataAnnotations;

namespace EShop.Models.PostModels
{
    public class ProductToCartPostModel
    {
        public int ProductId { get; set; }

        [Range(1, 100, ErrorMessage = "Amount of products cannot be less than 1 or greater than 100.")]
        public int Quantity { get; set; }
    }
}