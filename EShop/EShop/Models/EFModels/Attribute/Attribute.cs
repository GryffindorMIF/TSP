using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.Models.EFModels.Attribute
{
    public class Attribute : IEquatable<Attribute>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public string IconUrl { get; set; }

        public bool Equals(Attribute other)
        {
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(IconUrl, other.IconUrl, StringComparison.OrdinalIgnoreCase);
        }
    }
}