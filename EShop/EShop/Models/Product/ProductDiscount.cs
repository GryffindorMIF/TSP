using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    public class ProductDiscount: IEquatable<ProductDiscount>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "The price is required.")]
        [Range(0.01, 1000000, ErrorMessage = "Price should not be less than 0.01")]
        public decimal DiscountPrice { get; set; }

        [Required(ErrorMessage = "End date of discount is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime Ends { get; set; }

        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public bool Equals(ProductDiscount other)
        {
            return ProductId == other.ProductId;
        }
    }
}

