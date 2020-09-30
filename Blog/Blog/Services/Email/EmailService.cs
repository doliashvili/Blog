using Blog.Configuration;
using Blog.Helpers;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly SmtpClient _client;

        public EmailService(IOptions<SmtpSettings> options)
        {
            _smtpSettings = options.Value;
            _client = new SmtpClient(_smtpSettings.Server)
            {
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
            };
        }
        public async Task SendEmail(string email, string subject, string message)
        {
            var mailMessage = new MailMessage(
                _smtpSettings.From,
                email,
                subject,
                message
                );

            await _client.SendMailAsync(mailMessage);
        }

        public async Task SendEmailBySendGrid(string email, string subject, string message)
        {
            var client = new SendGridClient(ApiMail.apiKey);
            var from = new EmailAddress(_smtpSettings.From);
            var to = new EmailAddress(email);
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = $"<strong>{message}</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);

        }
    }
}
