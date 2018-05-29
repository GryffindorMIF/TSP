using System;
using System.Linq;
using System.Threading.Tasks;
using EShop.Business.Interfaces;
using EShop.Data;
using EShop.Models.EFModels.Order;
using EShop.Models.EFModels.User;

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

        public async Task<DeliveryAddress> FindAddressByZipcodeAsync(string zipcode)
        {
            DeliveryAddress address = null;
            await Task.Run(() =>
            {
                address = _context.DeliveryAddress.FirstOrDefault(da => da.Zipcode == zipcode);
            });
            return address;
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

        public async Task CreateDeliveryAddress(DeliveryAddress deliveryAddress)
        {
            _context.DeliveryAddress.Add(deliveryAddress);
            await _context.SaveChangesAsync();
        }

        public async Task<int> RemoveDeliveryAddressAsync(ApplicationUser user, DeliveryAddress addressOnDeathrow)
        {
            var returnCode = 1;

            await Task.Run(() =>
            {
                try
                {
                    var deliveryAddress = _context.DeliveryAddress.FirstOrDefault(da => da.Zipcode == addressOnDeathrow.Zipcode && da.User.Id == user.Id);

                    _context.Remove(deliveryAddress ?? throw new InvalidOperationException());

                    var t2 = Task.Run(
                        async () => { await _context.SaveChangesAsync(); });
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