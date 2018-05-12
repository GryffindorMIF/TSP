﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    public class ProductProperty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "The property is required.")]
        [MaxLength(20, ErrorMessage = "Property name cannot be longer than 20 symbols.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The description is required.")]
        [MaxLength(200, ErrorMessage = "Description cannot be longer than 200 symbols.")]
        public string Description { get; set; }
        
        public int ProductId { get; set; }

        [Required]
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
