using EShop.Business.Interfaces;
using EShop.Data;
using EShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddOrderReviewAsync(Order order, OrderReviewModel review)
        {
            int returnCode = 1;
            try
            {
                _context.OrderReview.Add(review);
                order.StatusCode = 4; //Reviewed
                _context.Update(order);
                await _context.SaveChangesAsync();

                returnCode = 0;
            }
            catch (Exception)
            {
                returnCode = 1;
            }
            return returnCode;
        }

        public async Task<Order> FindOrderByIdAsync(int Id)
        {
            Order order = null;
            order = await _context.Order.FindAsync(Id);
            return order;
        }

        public async Task<IQueryable<Order>> QueryAllOrdersAsync(ApplicationUser user)
        {
            IQueryable<Order> savedOrders = null;
            if (user != null)
                await Task.Run(() =>
                {
                    savedOrders = from o in _context.Order
                                  where o.User.Id == user.Id
                                  select new Order
                                  {
                                      Id = o.Id,
                                      ShoppingCartId = o.ShoppingCartId,
                                      TotalPrice = o.TotalPrice,
                                      Address = o.Address,
                                      User = user,
                                      CardNumber = o.CardNumber,
                                      PurchaseDate = o.PurchaseDate,
                                      ConfirmationDate = o.ConfirmationDate,
                                      StatusCode = o.StatusCode
                                  };
                });
            return (savedOrders.ToList().AsQueryable());
        }

        public async Task<IQueryable<Order>> QueryAllAdminOrdersAsync()
        {
            IQueryable<Order> savedOrders = null;
            await Task.Run(() =>
            {
                savedOrders = from o in _context.Order
                              select new Order
                              {
                                  Id = o.Id,
                                  ShoppingCartId = o.ShoppingCartId,
                                  TotalPrice = o.TotalPrice,
                                  Address = o.Address,
                                  User = o.User,
                                  CardNumber = o.CardNumber,
                                  PurchaseDate = o.PurchaseDate,
                                  ConfirmationDate = o.ConfirmationDate,
                                  StatusCode = o.StatusCode
                              };
            });
            return (savedOrders.ToList().AsQueryable());
        }

        public async Task<int> ChangeOrderConfirmationAsync(int Id, bool confirm)
        {
            int returnCode = 1;

            try
            {
                Order order = null;
                order = await _context.Order.FindAsync(Id);

                if (confirm) { order.StatusCode = 2; }
                else { order.StatusCode = 3; }

                order.ConfirmationDate = DateTime.Now;

                _context.Update(order);
                await _context.SaveChangesAsync();

                returnCode = 0;
            }
            catch (Exception)
            {
                returnCode = 1;
            }
            return returnCode;
        }

        public async Task<int> CreateOrderAsync(Order order)
        {
            int returnCode = 1;
            try
            {
                _context.Order.Add(order);
                await _context.SaveChangesAsync();
                returnCode = 0;
            }
            catch (Exception)
            {
                returnCode = 1;
            }
            return returnCode;
        }

        public async Task<OrderReviewModel> FindOrderReviewAsync(int OrderId)
        {
            OrderReviewModel orderReview = null;
            orderReview = await _context.OrderReview.Where(or => or.OrderId == OrderId).FirstOrDefaultAsync();
            return orderReview;
        }
    }
}
