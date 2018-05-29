using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EShop.Models.EFModels.Attribute;

namespace EShop.Models.EFModels.Product
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

        [Required] [ForeignKey("ProductId")] public virtual Product Product { get; set; }

        public bool Equals(ProductAttributeValue other)
        {
            return AttributeValueId == other.AttributeValueId && ProductId == other.ProductId;
        }
    }
}