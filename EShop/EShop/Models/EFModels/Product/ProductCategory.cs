using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.Models.EFModels.Product
{
    public class ProductCategory : IEquatable<ProductCategory>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProductId { get; set; }

        [ForeignKey("ProductId")] public virtual Product Product { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")] public virtual Category.Category Category { get; set; }

        public bool Equals(ProductCategory other)
        {
            return ProductId == other.ProductId && CategoryId == other.CategoryId;
        }
    }
}