using Resend;

namespace Pizza_API.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSenderService> _logger;
        private readonly IResend _resend;

        public EmailSenderService(IConfiguration configuration, ILogger<EmailSenderService> logger, IResend resend)
        {
            _configuration = configuration;
            _logger = logger;
            _resend = resend;
        }

        // SendEmailAsync
        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var message = new EmailMessage
            {
                From = $"{_configuration["Resend:FromName"]} <{_configuration["Resend:FromEmail"]}>",
                To = { toEmail },
                Subject = subject,
                HtmlBody = htmlMessage,
            };

            await _resend.EmailSendAsync(message);
            _logger.LogInformation($"Email sent successfully to {toEmail}");
        }
    }
}