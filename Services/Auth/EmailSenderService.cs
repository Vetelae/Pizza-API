using SendGrid;
using SendGrid.Helpers.Mail;

namespace Pizza_API.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSenderService> _logger;

        public EmailSenderService(IConfiguration configuration, ILogger<EmailSenderService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // SendEmailAsync
        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var apiKey = _configuration["SendGrid:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogError("SendGrid API key is not configured");
                throw new InvalidOperationException("SendGrid API key is not configured");
            }

            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(
                _configuration["SendGrid:FromEmail"],
                _configuration["SendGrid:FromName"]
                );

            var to = new EmailAddress(toEmail);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlMessage);

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode != System.Net.HttpStatusCode.OK &&
                response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                _logger.LogError($"Failed to send email to {toEmail}. Status: {response.StatusCode}");
                throw new Exception($"Failed to send email: {response.StatusCode}");
            }

            _logger.LogInformation($"Email sent successfully to {toEmail}");
        }
    }
}