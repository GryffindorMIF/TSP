using System.Collections.Generic;
using System.Linq;

namespace EShop.Models
{
    public class OrderHistoryModel
    {
        public ICollection<Order> Orders { get; set; }
        public List<OrderReview> Reviews { get; set; }
    }
}
