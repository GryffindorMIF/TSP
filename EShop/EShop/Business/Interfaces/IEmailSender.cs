using System.Threading.Tasks;

namespace EShop.Business.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}