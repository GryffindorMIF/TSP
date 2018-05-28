using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.Models.EFModels.ShoppingCart
{
    public class ShoppingCartProduct
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [Required] [ForeignKey("ProductId")] public virtual Product.Product Product { get; set; }

        public int ShoppingCartId { get; set; }

        [Required]
        [ForeignKey("ShoppingCartId")]
        public virtual ShoppingCart ShoppingCart { get; set; }

        [Range(1, 100, ErrorMessage = "Amount of products cannot be less than 1 or greater than 100.")]
        public int Quantity { get; set; }
    }
}