using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("ShoppingCartId")]
        public int? ShoppingCartId { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        public string CardNumber { get; set; }

        public DateTime PurchaseDate { get; set; }

        public DateTime ConfirmationDate { get; set; }

        [Required]
        public int StatusCode { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
