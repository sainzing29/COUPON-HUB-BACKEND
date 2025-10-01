using CouponHub.Business.Interfaces;
using CouponHub.DataAccess.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace CouponHub.Business.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendPasswordSetupEmailAsync(User user, string token)
        {
            try
            {
                var frontendUrl = _configuration["Frontend:BaseUrl"] ?? "https://yourfrontend.com";
                var setupUrl = $"{frontendUrl}/set-password?token={token}";

                var subject = "Welcome to CouponHub - Set Your Password";
                var body = GeneratePasswordSetupEmailBody(user, setupUrl);

                return await SendEmailAsync(user.Email, user.FirstName, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password setup email to {Email}", user.Email);
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(User user, string token)
        {
            try
            {
                var frontendUrl = _configuration["Frontend:BaseUrl"] ?? "https://yourfrontend.com";
                var resetUrl = $"{frontendUrl}/reset-password?token={token}";

                var subject = "CouponHub - Password Reset Request";
                var body = GeneratePasswordResetEmailBody(user, resetUrl);

                return await SendEmailAsync(user.Email, user.FirstName, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email to {Email}", user.Email);
                return false;
            }
        }

        public async Task<bool> SendWelcomeEmailAsync(User user)
        {
            try
            {
                var subject = "Welcome to CouponHub!";
                var body = GenerateWelcomeEmailBody(user);

                return await SendEmailAsync(user.Email, user.FirstName, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to {Email}", user.Email);
                return false;
            }
        }

        private async Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string body)
        {
            try
            {
                var smtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var smtpPort = _configuration.GetValue<int>("Email:SmtpPort", 587);
                var smtpUsername = _configuration["Email:SmtpUsername"];
                var smtpPassword = _configuration["Email:SmtpPassword"];
                var fromEmail = _configuration["Email:FromEmail"] ?? "noreply@couponhub.com";
                var fromName = _configuration["Email:FromName"] ?? "CouponHub Team";

                // Console logging for debugging
                Console.WriteLine("=== SMTP Configuration Debug ===");
                Console.WriteLine($"SMTP Host: {smtpHost}");
                Console.WriteLine($"SMTP Port: {smtpPort}");
                Console.WriteLine($"SMTP Username: {smtpUsername}");
                Console.WriteLine($"SMTP Password: {(string.IsNullOrEmpty(smtpPassword) ? "NOT SET" : "***SET***")}");
                Console.WriteLine($"From Email: {fromEmail}");
                Console.WriteLine($"From Name: {fromName}");
                Console.WriteLine($"To Email: {toEmail}");
                Console.WriteLine($"To Name: {toName}");
                
                // Debug: Show all Email configuration keys
                Console.WriteLine("--- All Email Configuration Keys ---");
                var emailSection = _configuration.GetSection("Email");
                foreach (var item in emailSection.GetChildren())
                {
                    Console.WriteLine($"Email:{item.Key} = {item.Value}");
                }
                Console.WriteLine("================================");

                if (string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
                {
                    _logger.LogWarning("SMTP credentials not configured. Email will not be sent.");
                    Console.WriteLine("ERROR: SMTP credentials not configured!");
                    return false;
                }

                using var client = new SmtpClient(smtpHost, smtpPort);
                client.EnableSsl = true;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                
                // Additional Gmail-specific settings
                client.Timeout = 10000; // 10 seconds timeout

                var message = new MailMessage();
                message.From = new MailAddress(fromEmail, fromName);
                message.To.Add(new MailAddress(toEmail, toName));
                message.Subject = subject;
                message.Body = body;
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;
                message.SubjectEncoding = Encoding.UTF8;

                await client.SendMailAsync(message);
                
                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
                Console.WriteLine($"SUCCESS: Email sent to {toEmail}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                Console.WriteLine($"ERROR: Failed to send email to {toEmail}");
                Console.WriteLine($"Exception Type: {ex.GetType().Name}");
                Console.WriteLine($"Exception Message: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                return false;
            }
        }

        private string GeneratePasswordSetupEmailBody(User user, string setupUrl)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Welcome to CouponHub</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 0 0 5px 5px; }}
        .button {{ display: inline-block; background-color: #4CAF50; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Welcome to CouponHub!</h1>
        </div>
        <div class='content'>
            <h2>Hello {user.FirstName}!</h2>
            <p>Welcome to CouponHub! Your account has been created successfully.</p>
            <p>To complete your account setup, please set your password by clicking the button below:</p>
            <p style='text-align: center;'>
                <a href='{setupUrl}' class='button'>Set Your Password</a>
            </p>
            <p><strong>Important:</strong></p>
            <ul>
                <li>This link will expire in 7 days</li>
                <li>You can only use this link once</li>
                <li>If you didn't request this account, please ignore this email</li>
            </ul>
            <p>If the button doesn't work, you can copy and paste this link into your browser:</p>
            <p style='word-break: break-all; background-color: #e9e9e9; padding: 10px; border-radius: 3px;'>{setupUrl}</p>
        </div>
        <div class='footer'>
            <p>This email was sent from CouponHub. Please do not reply to this email.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GeneratePasswordResetEmailBody(User user, string resetUrl)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Password Reset - CouponHub</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #f44336; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 0 0 5px 5px; }}
        .button {{ display: inline-block; background-color: #f44336; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Password Reset Request</h1>
        </div>
        <div class='content'>
            <h2>Hello {user.FirstName}!</h2>
            <p>We received a request to reset your password for your CouponHub account.</p>
            <p>To reset your password, please click the button below:</p>
            <p style='text-align: center;'>
                <a href='{resetUrl}' class='button'>Reset Password</a>
            </p>
            <p><strong>Important:</strong></p>
            <ul>
                <li>This link will expire in 7 days</li>
                <li>You can only use this link once</li>
                <li>If you didn't request this reset, please ignore this email</li>
            </ul>
            <p>If the button doesn't work, you can copy and paste this link into your browser:</p>
            <p style='word-break: break-all; background-color: #e9e9e9; padding: 10px; border-radius: 3px;'>{resetUrl}</p>
        </div>
        <div class='footer'>
            <p>This email was sent from CouponHub. Please do not reply to this email.</p>
        </div>
    </div>
</body>
</html>";
        }

        private string GenerateWelcomeEmailBody(User user)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Welcome to CouponHub</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 30px; border-radius: 0 0 5px 5px; }}
        .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Welcome to CouponHub!</h1>
        </div>
        <div class='content'>
            <h2>Hello {user.FirstName}!</h2>
            <p>Welcome to CouponHub! Your account has been successfully created and activated.</p>
            <p>You can now:</p>
            <ul>
                <li>Access your dashboard</li>
                <li>Manage coupons and services</li>
                <li>View analytics and reports</li>
                <li>Update your profile settings</li>
            </ul>
            <p>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
            <p>Thank you for choosing CouponHub!</p>
        </div>
        <div class='footer'>
            <p>This email was sent from CouponHub. Please do not reply to this email.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
