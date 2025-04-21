using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using GestorAlquilerVehiculos.Models;

namespace GestorAlquilerVehiculos.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailService(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }
        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_emailConfig.From);
            email.From.Add(MailboxAddress.Parse(_emailConfig.From));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            var builder = new BodyBuilder();
            if (isHtml)
            {
                builder.HtmlBody = body;
            }
            else
            {
                builder.TextBody = body;
            }
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();

            smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

            try
            {
                await smtp.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.SslOnConnect);
            }
            catch (Exception)
            {
                await smtp.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);
            }

            await smtp.AuthenticateAsync(_emailConfig.Username, _emailConfig.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath, bool isHtml = true)
        {
            var email = new MimeMessage();

            email.Sender = MailboxAddress.Parse(_emailConfig.From);
            email.From.Add(MailboxAddress.Parse(_emailConfig.From));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            var builder = new BodyBuilder();
            if (isHtml)
            {
                builder.HtmlBody = body;
            }
            else
            {
                builder.TextBody = body;
            }


            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailConfig.Username, _emailConfig.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
