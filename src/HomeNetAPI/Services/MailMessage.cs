using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;

namespace HomeNetAPI.Services
{
    public class MailMessage : IMailMessage
    {
        public bool SendMailMessage(String destinationName, String destinationEmail, String title, String message)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Okuhle Ngada (HomeNET System)", "okuhle.ngada@koeksworld.com"));
            email.To.Add(new MailboxAddress(destinationName, destinationEmail));
            email.Subject = title;
            email.Body = new TextPart("plain")
            {
                Text = message
            };
            var emailClient = new SmtpClient();
            emailClient.Connect("mail.koeksworld.com", 465, true);
            emailClient.Authenticate("okuhle.ngada@koeksworld.com", "Okuhle*1994");
            emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
            emailClient.Send(email);
            emailClient.Disconnect(true);
            return true;
        }
    }
}
