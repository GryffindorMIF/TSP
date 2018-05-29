using System.Threading.Tasks;
using EShop.Business.Interfaces;

namespace EShop.Extensions
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link or copying it to browser address field: {link}"); //<a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }

        public static Task SendEmailForgotPasswordAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Reset Password",
                $"Please reset your account password by clicking this link or copying it to browser address field: {link}");
        }
    }
}