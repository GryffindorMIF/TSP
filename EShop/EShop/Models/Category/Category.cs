using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }// unique
        public string Description { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual ICollection<ProductCategory> ProductCategories { get; set; }
    }
}
