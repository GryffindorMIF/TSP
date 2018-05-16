﻿using EShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business.Interfaces
{
    public interface IOrderService
    {
        Task<int> AddOrderReviewAsync(Order order, OrderReviewModel review);
        Task<int> ChangeOrderConfirmationAsync(int Id, bool confirm, byte[] rowVersion); //True if confirm, false if reject
        Task<int> CreateOrderAsync(Order order);
        Task<OrderReviewModel> FindOrderReviewAsync(int OrderId);
        Task<IQueryable<Order>> QueryAllOrdersAsync(ApplicationUser user);
        Task<IQueryable<Order>> QueryAllAdminOrdersAsync();
        Task<Order> FindOrderByIdAsync(int Id);
    }
}