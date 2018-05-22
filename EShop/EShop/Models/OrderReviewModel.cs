﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.Models
{
    public class OrderReviewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Rating { get; set; }

        public string CustomerComment { get; set; }

        [Required]
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        [ForeignKey("OrderId")]
        public int? OrderId { get; set; }
    }
}
