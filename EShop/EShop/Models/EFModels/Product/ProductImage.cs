﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.Models.EFModels.Product
{
    public class ProductImage : IEquatable<ProductImage>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }
        public int ProductId { get; set; }

        [Required] [ForeignKey("ProductId")] public virtual Product Product { get; set; }

        public bool Equals(ProductImage other)
        {
            return string.Equals(ImageUrl, other.ImageUrl, StringComparison.OrdinalIgnoreCase) &&
                   ProductId == other.ProductId && IsPrimary == other.IsPrimary;
        }
    }
}