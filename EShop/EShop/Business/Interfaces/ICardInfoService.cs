using System.Threading.Tasks;
using EShop.Models.EFModels.Order;

namespace EShop.Business.Interfaces
{
    public interface ICardInfoService
    {
        Task<CardInfo> GetCardInfoById(int cardInfoId);
        Task<CardInfo> GetCardInfoByUserId(string userId);
        Task<int> CreateCardInfo(CardInfo cardInfo);
        Task<int> UpdateCardInfo(CardInfo newCardInfo);
        Task<int> DeleteCardInfo(int cardInfoId);
    }
}