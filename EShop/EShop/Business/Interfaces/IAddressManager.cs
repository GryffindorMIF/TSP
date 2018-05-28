using System.Linq;
using System.Threading.Tasks;
using EShop.Models.EFModels.Order;
using EShop.Models.EFModels.User;

namespace EShop.Business.Interfaces
{
    public interface IAddressManager
    {
        Task<IQueryable<DeliveryAddress>> QueryAllSavedDeliveryAddresses(ApplicationUser user);
        Task CreateDeliveryAddress(DeliveryAddress deliveryAddress);
        Task<int> RemoveDeliveryAddressAsync(ApplicationUser user, DeliveryAddress addressOnDeathrow);
        Task<DeliveryAddress> FindAddressByZipcodeAsync(string zipcode);
    }
}