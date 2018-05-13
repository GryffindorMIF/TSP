using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    public class ProductImage : IEquatable<ProductImage>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }
        public int ProductId { get; set; }

        [Required]
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public bool Equals(ProductImage other)
        {
            return string.Equals(ImageUrl, other.ImageUrl, StringComparison.OrdinalIgnoreCase);
        }
    }
}
