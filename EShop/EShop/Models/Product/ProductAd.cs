﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    public class ProductAd
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string AdImageUrl { get; set; }
        public int ProductId { get; set; }
        [Required]
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}