using LaptopStoreProject_MVC.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System.IO;

namespace LaptopStoreProject_MVC.Services
{
    // Cấu hình dịch vụ gửi mail, giá trị Inject từ appsettings.json
    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }

    // Dịch vụ gửi mail
    public class SendMailService : IEmailSender
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<SendMailService> _logger;

        // Inject MailSettings và Logger
        public SendMailService(IOptions<MailSettings> mailSettings, ILogger<SendMailService> logger)
        {
            _mailSettings = mailSettings.Value ?? throw new ArgumentNullException(nameof(mailSettings));
            _logger = logger;
            _logger.LogInformation("Create SendMailService");
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Email address is null or empty. Skipping email sending.");
                return;
            }

            var message = new MimeMessage();
            message.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
            message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = htmlMessage };
            message.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                _logger.LogInformation("Connecting to SMTP server...");
                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                _logger.LogInformation("Authenticating SMTP server...");
                await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(message);

                _logger.LogInformation($"Email sent successfully to {email}");
            }
            catch (SmtpCommandException smtpEx)
            {
                _logger.LogError($"SMTP command error: {smtpEx.Message}. Code: {smtpEx.StatusCode}");
                SaveEmailToFile(message);
            }
            catch (SmtpProtocolException protocolEx)
            {
                _logger.LogError($"SMTP protocol error: {protocolEx.Message}");
                SaveEmailToFile(message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"General error while sending email: {ex.Message}");
                SaveEmailToFile(message);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
                _logger.LogInformation("Disconnected from SMTP server.");
            }
        }

        // Lưu email vào tệp nếu xảy ra lỗi
        private void SaveEmailToFile(MimeMessage message)
        {
            try
            {
                var mailSaveDir = Path.Combine(Directory.GetCurrentDirectory(), "mailssave");
                Directory.CreateDirectory(mailSaveDir);

                var emailSaveFile = Path.Combine(mailSaveDir, $"{Guid.NewGuid()}.eml");
                using var stream = File.Create(emailSaveFile);
                message.WriteTo(stream);

                _logger.LogInformation($"Failed email saved to: {emailSaveFile}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to save email to file: {ex.Message}");
            }
        }
    }
}
