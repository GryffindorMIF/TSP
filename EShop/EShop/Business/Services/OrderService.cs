﻿using EShop.Business.Interfaces;
using EShop.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using EShop.Models.EFModels.Order;
using EShop.Models.EFModels.User;

namespace EShop.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddOrderReviewAsync(Order order, OrderReview review)
        {
            int returnCode;
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

        public async Task<Order> FindOrderByIdAsync(int id)
        {
            var order = await _context.Order.FindAsync(id);
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
                                      StatusCode = o.StatusCode,
                                      RowVersion = o.RowVersion
                                  };
                });
            return (((savedOrders ?? throw new InvalidOperationException()) ?? throw new InvalidOperationException()).ToList().AsQueryable());
        }

        public ICollection<Order> GetAllOrdersByPage(ApplicationUser user, int pageNumber, int ordersPerPage)
        {
            if (user != null)
            {
                ICollection<Order> orders = (from o in _context.Order
                    where o.User.Id == user.Id
                    select o).Skip(pageNumber * ordersPerPage).Take(ordersPerPage).ToList();
                return orders;
            }
            else throw new ArgumentException();
        }

        public int GetOrdersPageCount(ApplicationUser user, int ordersPerPage)
        {
            var ordersTotalCount = _context.Order.Where(o => o.User.Id == user.Id).Count();

            var pageCount = ordersTotalCount / ordersPerPage;

            if (ordersTotalCount % ordersPerPage != 0)
            {
                pageCount++;
            }

            return pageCount;
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
                                  StatusCode = o.StatusCode,
                                  RowVersion = o.RowVersion
                              };
            });
            return (savedOrders.ToList().AsQueryable());
        }

        public ICollection<Order> GetAllAdminOrdersByPage(int pageNumber, int ordersPerPage)
        {
            return _context.Order.Skip(pageNumber * ordersPerPage).Take(ordersPerPage).ToList();
        }

        public int GetAdminOrdersPageCount(int ordersPerPage)
        {
            var ordersTotalCount = _context.Order.Count();

            var pageCount = ordersTotalCount / ordersPerPage;

            if (ordersTotalCount % ordersPerPage != 0)
            {
                pageCount++;
            }
            return pageCount;
        }

        public async Task<int> ChangeOrderConfirmationAsync(int id, bool confirm, byte[] rowVersion)
        {
            int returnCode;

            try
            {
                var order = await _context.Order.FindAsync(id);

               _context.Entry(order).Property("RowVersion").OriginalValue = rowVersion;

                if (confirm) { order.StatusCode = 2; }
                else { order.StatusCode = 3; }

                order.ConfirmationDate = DateTime.Now;

                _context.Update(order);
                await _context.SaveChangesAsync();

                returnCode = 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                returnCode = -1;
            }
            catch (Exception)
            {
                returnCode = 1;
            }
            return returnCode;
        }

        public async Task<int> CreateOrderAsync(Order order)
        {
            int returnCode;
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

        public async Task<OrderReview> FindOrderReviewAsync(int orderId)
        {
            var orderReview = await _context.OrderReview.Where(or => or.OrderId == orderId).FirstOrDefaultAsync();
            return orderReview;
        }

        public async Task<int> Purchase(int totalCost, string postMessage)
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                PreAuthenticate = true,
                UseDefaultCredentials = false,
            };

            HttpClient client = new HttpClient(handler);
            client.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{"technologines"}:{"platformos"}")));

            //Create POST content from data
            StringContent jContent = new StringContent(postMessage, Encoding.UTF8, "application/json");
            jContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "https://mock-payment-processor.appspot.com/v1/payment/")
            {
                Content = jContent,
            };

            HttpResponseMessage response = await client.SendAsync(req);

            return (int)response.StatusCode;
        }
    }
}
