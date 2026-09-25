using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace FootBallShop.Service
{
    public class EmailSenderService : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailSenderService> _logger;

        public EmailSenderService(IConfiguration config, ILogger<EmailSenderService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var senderEmail = _config["Email__SenderEmail"] ?? Environment.GetEnvironmentVariable("Email__SenderEmail");
            var senderPassword = _config["Email__SenderPassword"] ?? Environment.GetEnvironmentVariable("Email__SenderPassword");
            var smtpHost = _config["Email__SmtpHost"] ?? Environment.GetEnvironmentVariable("Email__SmtpHost") ?? "smtp.gmail.com";
            var smtpPortStr = _config["Email__SmtpPort"] ?? Environment.GetEnvironmentVariable("Email__SmtpPort") ?? "587";

            if (string.IsNullOrWhiteSpace(senderEmail) || string.IsNullOrWhiteSpace(senderPassword))
            {
                _logger.LogWarning("Email not configured — skipping send to {Email} | Subject: {Subject}", email, subject);
                return;
            }

            try
            {
                var smtpClient = new SmtpClient(smtpHost)
                {
                    Port = int.Parse(smtpPortStr),
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, "FootBallShop"),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent to {Email} — Subject: {Subject}", email, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email} — Subject: {Subject}", email, subject);
                
            }
        }
    }
}