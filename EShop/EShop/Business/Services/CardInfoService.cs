using System.Linq;
using System.Threading.Tasks;
using EShop.Business.Interfaces;
using EShop.Data;
using EShop.Models;
using Microsoft.EntityFrameworkCore;

namespace EShop.Business.Services
{
    public class CardInfoService : ICardInfoService
    {
        private readonly ApplicationDbContext _context;

        public CardInfoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateCardInfo(CardInfo cardInfo)
        {
            int resultCode = 1; //Error
            if (cardInfo != null)
            {
                try
                {
                    await _context.AddAsync(cardInfo);
                    await _context.SaveChangesAsync();
                    resultCode = 0; //Success
                }
                catch (System.Exception)
                {
                    resultCode = 1;
                }
            }
            return resultCode;
        }

        public async Task<int> DeleteCardInfo(int cardInfoId)
        {
            int resultCode = 1; //Error
            CardInfo cardInfo = await GetCardInfoById(cardInfoId);
            if (cardInfo != null)
            {
                try
                {
                    _context.Remove(cardInfo);
                    await _context.SaveChangesAsync();
                    resultCode = 0; //Success
                }
                catch (System.Exception)
                {
                    resultCode = 1;
                }
            }
            return resultCode;
        }

        public async Task<CardInfo> GetCardInfoById(int cardInfoId)
        {
            return await _context.CardInfo.FindAsync(cardInfoId);
        }

        public async Task<CardInfo> GetCardInfoByUserId(string userId)
        {
            return await _context.CardInfo.Where(c => c.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<int> UpdateCardInfo(CardInfo newCardInfo)
        {
            int resultCode = 1; //Error
            if (newCardInfo != null)
            {
                try
                {
                    _context.Update(newCardInfo);
                    await _context.SaveChangesAsync();
                    resultCode = 0; //Success
                }
                catch (System.Exception)
                {
                    resultCode = 1;
                }
            }
            return resultCode;
        }
    }
}
