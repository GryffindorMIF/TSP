using EShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business.Interfaces
{
    public interface IAddressManager
    {
        Task<IQueryable<DeliveryAddress>> QueryAllSavedDeliveryAddresses(ApplicationUser user);
        Task<int> RemoveDeliveryAddressAsync(ApplicationUser user, DeliveryAddress addressOnDeathrow);
    }
}
