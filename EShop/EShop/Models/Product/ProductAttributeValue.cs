using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    public class ProductAttributeValue : IEquatable<ProductAttributeValue>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AttributeValueId { get; set; }
        public int ProductId { get; set; }

        [Required]
        [ForeignKey("AttributeValueId")]
        public virtual AttributeValue AttributeValue { get; set; }

        [Required]
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public bool Equals(ProductAttributeValue other)
        {
            return AttributeValueId == other.AttributeValueId && ProductId == other.ProductId;
        }
    }
}
