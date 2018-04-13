using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    public class ProductDetails
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "The property is required.")]
        [MaxLength(200)]
        public string Property { get; set; }
        [Required(ErrorMessage = "The description is required.")]
        [MaxLength(200)]
        public string Description { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
    }
}
