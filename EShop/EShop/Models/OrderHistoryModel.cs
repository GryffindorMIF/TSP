using System.Collections.Generic;

namespace EShop.Models
{
    public class OrderHistoryModel
    {
        public IEnumerable<Order> Orders { get; set; }

        public int Rating { get; set; }

        public string ExtraComments { get; set; }
    }
}
