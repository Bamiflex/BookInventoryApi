
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace SqlBasedBookAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendWelcomeEmail(string toEmail, string username)
        {
            if (string.IsNullOrWhiteSpace(toEmail) || string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Email or username cannot be null or empty.");
            }

            var fromEmail = _config["EmailSettings:FromEmail"];
            if (string.IsNullOrEmpty(fromEmail))
            {
                throw new InvalidOperationException("Sender email cannot be null or empty.");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Dreamville", fromEmail));
            message.To.Add(new MailboxAddress(username, toEmail));
            message.Subject = "Welcome to Dreamville!";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
<html>
<body style='font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: white; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px; background-color: white; border-radius: 8px;'>
        <div style='text-align: center;'>
            <img src='cid:logoImage' alt='Logo' style='max-width: 100px; height: auto;' />
        </div>
        <hr style='border: 2px solid #1D2E37;' />
        <div style='font-size: 16px; line-height: 1.5; color: #333;'>
            <p>Hello {username}, welcome to Dreamville!</p>
            <p>We're excited to have you join our community of book lovers. Get ready to explore our world and show off your book collections that will inspire, entertain, and enlighten the world.</p>
            <p style='font-style: italic; text-align: center;'>...Your Power House of Books</p>
        </div>
        <hr style='border: 1px solid #ddd;' />
        <div style='text-align: center; color: gray; font-size: 12px;'>
            <p>If you have any questions, feel free to reach out to our support team at <a href='mailto:support@dreamville.com' style='color: #1D2E37;'>support@dreamville.com</a>.</p>
            <p><strong>Dreamville Team</strong><br>&copy; 2024 Dreamville. All rights reserved.</p>
        </div>
    </div>
</body>
</html>"
            };


            var logoPath = "Image.png";
            var logoImage = bodyBuilder.LinkedResources.Add(logoPath);
            logoImage.ContentId = "logoImage";


            message.Body = bodyBuilder.ToMessageBody();

            Console.WriteLine($"Sending email to: {toEmail}, Username: {username}");

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:SmtpPort"]), MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_config["EmailSettings:SmtpUser"], _config["EmailSettings:SmtpPass"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}