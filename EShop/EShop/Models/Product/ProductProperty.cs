using System;
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
        [MaxLength(20)]
        public string Name { get; set; }
        [Required(ErrorMessage = "The description is required.")]
        [MaxLength(200)]
        public string Description { get; set; }
        [Required]
        [ForeignKey("Product")]
        public int ProductId { get; set; }
    }
}
