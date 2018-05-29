using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EShop.Models.EFModels.Product;

namespace EShop.Models.EFModels.Category
{
    public class Category : IEquatable<Category>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; } // unique
        public string Description { get; set; }

        [Timestamp] public byte[] RowVersion { get; set; }

        public virtual ICollection<ProductCategory> ProductCategories { get; set; }

        public virtual CategoryCategory CategoryCategory { get; set; }

        public bool Equals(Category other)
        {
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(Description, other.Description, StringComparison.OrdinalIgnoreCase);
        }
    }
}