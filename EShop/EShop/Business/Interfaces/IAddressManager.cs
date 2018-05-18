using EShop.Models;
using System.Linq;
using System.Threading.Tasks;

namespace EShop.Business.Interfaces
{
    public interface IAddressManager
    {
        Task<IQueryable<DeliveryAddress>> QueryAllSavedDeliveryAddresses(ApplicationUser user);
        Task CreateDeliveryAddress(DeliveryAddress deliveryAddress);
        Task<int> RemoveDeliveryAddressAsync(ApplicationUser user, DeliveryAddress addressOnDeathrow);
        Task<DeliveryAddress> FindAddressByZipcodeAsync(string Zipcode);
    }
}
