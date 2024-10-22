
using System.Threading.Tasks;

namespace SqlBasedBookAPI.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmail(string toEmail, string username);
    }
}


/*
 using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SqlBasedBookAPI.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string username);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string username)
        {
            // Get email settings from configuration
            var fromEmail = _config["EmailSettings:FromEmail"];
            var smtpServer = _config["EmailSettings:SMTPServer"];
            var smtpPort = int.Parse(_config["EmailSettings:SMTPPort"]);
            var smtpUser = _config["EmailSettings:SMTPUser"];
            var smtpPass = _config["EmailSettings:SMTPPassword"];

            // Create the email message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Dreamville", fromEmail));
            message.To.Add(new MailboxAddress(username, toEmail));
            message.Subject = "Welcome to Dreamville!";

            // Create email body (HTML content)
            message.Body = new TextPart("html")
            {
                Text = $@"
                <html>
                    <body>
                        <h1>Hello {username}, welcome to Dreamville!</h1>
                        <p>We're excited to have you join our community of book lovers.</p>
                        <p>Get ready to explore our world and share your book collections!</p>
                        <p>...Your Power House of Books</p>
                    </body>
                </html>"
            };

            try
            {
                // Set up the SMTP client
                using (var smtpClient = new SmtpClient())
                {
                    // Connect to the SMTP server
                    await smtpClient.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);

                    // Authenticate with the SMTP server
                    await smtpClient.AuthenticateAsync(smtpUser, smtpPass);

                    // Send the email
                    await smtpClient.SendAsync(message);

                    // Disconnect from the SMTP server
                    await smtpClient.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                // For now, rethrow the exception with a custom message
                throw new InvalidOperationException("Failed to send email.", ex);
            }
        }
    }
}

*/
