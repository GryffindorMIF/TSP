using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.Models
{
    public class CardInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("UserId")]
        public string UserId { get; set; }

        [CreditCard, Required]
        public string CardNumber { get; set; }

        [Required]
        public string ExpYear { get; set; }

        [Required]
        public string ExpMonth { get; set; }
    }
}
