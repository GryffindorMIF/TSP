using System.Threading.Tasks;
using EShop.Business.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace EShop.Business.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private readonly string _login;
        private readonly string _password;

        public EmailSender(IConfiguration configuration)
        {
            _login = configuration["EmailCredentials:Login"];
            _password = configuration["EmailCredentials:Password"];
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            //From address
            var fromAddress = "gryffindor.shop@gmail.com";
            var fromAddressTitle = "Gryffindor E-shop";
            //To address
            var ToAdressTitle = "Dear Client";

            //SMTP Server
            var smtpServer = "smtp.gmail.com";
            //SMTP port number
            var smtpPort = 587;

            var mimeMessage = new MimeMessage();

            mimeMessage.From.Add(new MailboxAddress(fromAddressTitle, fromAddress));
            mimeMessage.To.Add(new MailboxAddress(ToAdressTitle, email));
            mimeMessage.Subject = subject;
            mimeMessage.Body = new TextPart("plain")
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(smtpServer, smtpPort, false);
                // Note: only needed if the SMTP server requires authentication  
                // Error 5.5.1 Authentication   
                await client.AuthenticateAsync(_login, _password);
                await client.SendAsync(mimeMessage);
                //Console.WriteLine("The mail has been sent successfully!");
                //Console.ReadLine();
                await client.DisconnectAsync(true);
                //return Task.CompletedTask;
            }
        }
    }
}