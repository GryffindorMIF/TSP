using System.Linq;

namespace EShop.Models
{
    public class OrderHistoryModel
    {
        public IQueryable<Order> Orders { get; set; }
    }
}
