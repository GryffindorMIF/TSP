using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Models.PostModels
{
    public class ReviewPostModel
    {
        public int OrderId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
