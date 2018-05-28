using System.Collections.Generic;
using EShop.Models.EFModels.Order;

namespace EShop.Models.ViewModels
{
    public class OrderHistoryModel
    {
        public ICollection<Order> Orders { get; set; }
        public List<OrderReview> Reviews { get; set; }
    }
}