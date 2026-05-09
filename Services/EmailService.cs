using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace CafeManagement.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST") ?? _config["EmailSettings:Host"];
            var smtpPortStr = Environment.GetEnvironmentVariable("SMTP_PORT") ?? _config["EmailSettings:Port"];
            var smtpUser = Environment.GetEnvironmentVariable("SMTP_USER") ?? _config["EmailSettings:Username"];
            var smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS") ?? _config["EmailSettings:Password"];
            var fromEmail = Environment.GetEnvironmentVariable("SMTP_FROM") ?? _config["EmailSettings:FromEmail"];

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser))
            {
                Console.WriteLine("Email settings not configured. Skipping email send.");
                return;
            }

            int smtpPort = int.Parse(smtpPortStr ?? "587");

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail ?? smtpUser),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
        public async Task SendLowStockEmailAsync(string itemName, decimal currentStock, string unit)
        {
            var adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "admin@cafe.com";
            var subject = $"⚠️ Low Stock Alert: {itemName}";
            var body = $@"
                <h3>Inventory Warning</h3>
                <p>The ingredient <strong>{itemName}</strong> has reached its minimum stock level.</p>
                <p>Current Quantity: {currentStock} {unit}</p>
                <p>Please restock soon to avoid service interruptions.</p>
            ";
            await SendEmailAsync(adminEmail, subject, body);
        }
    }
}
