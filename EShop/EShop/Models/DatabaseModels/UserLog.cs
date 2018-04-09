using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models.DatabaseModels
{
    public class UserLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string MethodExecuted { get; set; }
        public string MethodClass { get; set; }
        public DateTime DateTime { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
