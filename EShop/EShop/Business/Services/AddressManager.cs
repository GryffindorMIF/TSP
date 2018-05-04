using EShop.Business.Interfaces;
using EShop.Data;
using EShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business.Services
{
    //This whole thing needs to be polished
    public class AddressManager : IAddressManager
    {
        private readonly ApplicationDbContext _context;

        public AddressManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<DeliveryAddress>> QueryAllSavedDeliveryAddresses(ApplicationUser user)
        {
            IQueryable<DeliveryAddress> savedAddresses = null;
            if (user != null)
                await Task.Run(() =>
                {
                    savedAddresses = from da in _context.DeliveryAddress
                                     where da.User.Id == user.Id
                                     select new DeliveryAddress
                                     {
                                         Country = da.Country,
                                         County = da.County,
                                         City = da.City,
                                         Address = da.Address,
                                         Zipcode = da.Zipcode,
                                         User = da.User
                                     };
                });
            return savedAddresses;
        }

        public async Task<int> RemoveDeliveryAddressAsync(ApplicationUser user, DeliveryAddress addressOnDeathrow)
        {
            int returnCode = 1;

            await Task.Run(() =>
            {
                try
                {
                    DeliveryAddress deliveryAddress = _context.DeliveryAddress
                            .Where(da => da.Zipcode == addressOnDeathrow.Zipcode && da.User.Id == user.Id).FirstOrDefault();

                    _context.Remove(deliveryAddress);

                    var t2 = Task.Run(
                        async () =>
                        {
                            await _context.SaveChangesAsync();
                        });
                    t2.Wait();
                    returnCode = 0; //All is good
                }
                catch (Exception)
                {
                    returnCode = 1; //I am angery
                }
            });

            return returnCode;
        }
    }
}