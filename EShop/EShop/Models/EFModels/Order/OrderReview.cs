using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EShop.Models.EFModels.User;

namespace EShop.Models.EFModels.Order
{
    public class OrderReview
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Rating { get; set; }

        public string CustomerComment { get; set; }

        [Required] [ForeignKey("UserId")] public virtual ApplicationUser User { get; set; }

        [Required] [ForeignKey("OrderId")] public int? OrderId { get; set; }
    }
}