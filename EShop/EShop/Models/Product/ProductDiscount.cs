﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    public class ProductDiscount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public decimal DiscountPrice { get; set; }
        public DateTime Starts { get; set; }
        public DateTime Ends { get; set; }

        [Required]
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
