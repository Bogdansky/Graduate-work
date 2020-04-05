using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services
{
    public class EmailService
    {
        private static readonly string mailbox;
        private static readonly string password;

        static EmailService()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("appsettings.json");
            var config = builder.Build();
            mailbox = config.GetValue<string>("mailbox");
            password = config.GetValue<string>("password");
        }

        public async Task SendEmailAsync(string email, string subject, string message, string receiver = "")
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Планировщик задач", mailbox));
            emailMessage.To.Add(new MailboxAddress(receiver, email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.mail.ru", 25, false);
                await client.AuthenticateAsync(mailbox, password);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
