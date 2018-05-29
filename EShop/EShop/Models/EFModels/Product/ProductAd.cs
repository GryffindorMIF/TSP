using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.Models.EFModels.Product
{
    public class ProductAd : IEquatable<ProductAd>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string AdImageUrl { get; set; }
        public int ProductId { get; set; }

        [Required] [ForeignKey("ProductId")] public virtual Product Product { get; set; }

        public bool Equals(ProductAd other)
        {
            return string.Equals(AdImageUrl, other.AdImageUrl, StringComparison.OrdinalIgnoreCase) &&
                   ProductId == other.ProductId;
        }
    }
}