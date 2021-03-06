﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EShop.Models.EFModels.User;

namespace EShop.Models.EFModels.Order
{
    public class DeliveryAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required] [ForeignKey("UserId")] public virtual ApplicationUser User { get; set; }

        [Required] public string Address { get; set; }

        [Required] public string Country { get; set; }

        [Required] public string County { get; set; }

        [Required] public string City { get; set; }

        [Required] public string Zipcode { get; set; }
    }
}