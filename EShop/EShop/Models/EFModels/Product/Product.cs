﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.Models.EFModels.Product
{
    public class Product : IEquatable<Product>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [MaxLength(100, ErrorMessage = "Max {0} length is {1}")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required field.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price cannot be less than 0.01")]
        public decimal Price { get; set; }

        [Timestamp] public byte[] RowVersion { get; set; }

        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
        public virtual ProductDiscount ProductDiscount { get; set; }
        public virtual ProductAd ProductAd { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<ProductProperty> ProductProperies { get; set; }
        public virtual ICollection<ProductAttributeValue> ProductAttributeValues { get; set; }

        public bool Equals(Product other)
        {
            return string.Equals(Name.Trim(), other.Name.Trim(), StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(Description.Trim(), other.Description.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}