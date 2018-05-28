using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.Models.EFModels.Category
{
    public class CategoryCategory : IEquatable<CategoryCategory>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")] public virtual Category Category { get; set; }

        public int? ParentCategoryId { get; set; }

        [ForeignKey("ParentCategoryId")] public virtual Category ParentCategory { get; set; }

        public bool Equals(CategoryCategory other)
        {
            return CategoryId == other.CategoryId && ParentCategoryId == other.ParentCategoryId;
        }
    }
}