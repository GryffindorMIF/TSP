using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace EShop.Business
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private string login;
        private string password;

        public EmailSender(IConfiguration configuration)
        {
            login = configuration["EmailCredentials:Login"];
            password = configuration["EmailCredentials:Password"];
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                //From address
                string fromAddress = "gryffindor.shop@gmail.com";
                string fromAddressTitle = "Gryffindor E-shop";
                //To address
                string ToAdressTitle = "Dear Client";

                //SMTP Server
                string SmtpServer = "smtp.gmail.com";
                //SMTP port number
                int SmtpPort = 587;

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
                    await client.ConnectAsync(SmtpServer, SmtpPort, false);
                    // Note: only needed if the SMTP server requires authentication  
                    // Error 5.5.1 Authentication   
                    await client.AuthenticateAsync(login, password);
                    await client.SendAsync(mimeMessage);
                    //Console.WriteLine("The mail has been sent successfully!");
                    //Console.ReadLine();
                    await client.DisconnectAsync(true);
                    //return Task.CompletedTask;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
