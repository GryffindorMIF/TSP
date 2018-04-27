using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("ShoppingCartId")]
        public virtual ShoppingCart ShoppingCart { get; set; }

        [Required]
        public int TotalPrice { get; set; }

        [Required]
        [ForeignKey("AddressId")]
        public virtual DeliveryAddress Address { get; set; }

        [Required]
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        public string CardNumber { get; set; }

        public DateTime PurchaseDate { get; set; }

        public DateTime ConfirmationDate { get; set; }

        [Required]
        public int StatusCode { get; set; }
    }
}
