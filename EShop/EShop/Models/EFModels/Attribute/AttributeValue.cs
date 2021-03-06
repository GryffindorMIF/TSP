﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EShop.Models.EFModels.Attribute
{
    public class AttributeValue : IEquatable<AttributeValue>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public int AttributeId { get; set; }

        [ForeignKey("AttributeId")] public virtual Attribute Attribute { get; set; }

        public bool Equals(AttributeValue other)
        {
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) &&
                   AttributeId == other.AttributeId;
        }
    }
}