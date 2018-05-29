using System;
using System.Linq;
using System.Threading.Tasks;
using EShop.Business.Interfaces;
using EShop.Data;
using EShop.Models.EFModels.Order;
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
            var resultCode = 1; //Error
            if (cardInfo != null)
                try
                {
                    await _context.AddAsync(cardInfo);
                    await _context.SaveChangesAsync();
                    resultCode = 0; //Success
                }
                catch (Exception)
                {
                    resultCode = 1;
                }

            return resultCode;
        }

        public async Task<int> DeleteCardInfo(int cardInfoId)
        {
            var resultCode = 1; //Error
            var cardInfo = await GetCardInfoById(cardInfoId);
            if (cardInfo == null) return resultCode;
            try
            {
                _context.Remove(cardInfo);
                await _context.SaveChangesAsync();
                resultCode = 0; //Success
            }
            catch (Exception)
            {
                resultCode = 1;
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
            var resultCode = 1; //Error
            if (newCardInfo == null) return resultCode;
            try
            {
                _context.Update(newCardInfo);
                await _context.SaveChangesAsync();
                resultCode = 0; //Success
            }
            catch (Exception)
            {
                resultCode = 1;
            }

            return resultCode;
        }
    }
}