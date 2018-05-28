using EShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business.Interfaces
{
    public interface IOrderService
    {
        Task<int> AddOrderReviewAsync(Order order, OrderReview review);
        Task<int> ChangeOrderConfirmationAsync(int Id, bool confirm, byte[] rowVersion); //True if confirm, false if reject
        Task<int> CreateOrderAsync(Order order);
        Task<OrderReview> FindOrderReviewAsync(int OrderId);
        Task<IQueryable<Order>> QueryAllOrdersAsync(ApplicationUser user);
        ICollection<Order> GetAllOrdersByPage(ApplicationUser user, int pageNumber, int ordersPerPage);
        int GetOrdersPageCount(ApplicationUser user, int ordersPerPage);
        Task<IQueryable<Order>> QueryAllAdminOrdersAsync();
        ICollection<Order> GetAllAdminOrdersByPage(int pageNumber, int ordersPerPage);
        int GetAdminOrdersPageCount(int ordersPerPage);
        Task<Order> FindOrderByIdAsync(int Id);
        Task<int> Purchase(int totalCost, string postMessage);
    }
}
