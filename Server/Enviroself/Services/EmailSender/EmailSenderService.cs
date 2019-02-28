using Enviroself.Infrastructure.SendGrid;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Enviroself.Services.EmailSender
{
    public class EmailSenderService : IEmailSenderService
    {
        #region Fields
        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager
        private readonly IOptions<AuthMessageSenderOptions> optionsAccessor;
        #endregion

        #region Ctor
        public EmailSenderService(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
            this.optionsAccessor = optionsAccessor;
        }
        #endregion

        #region Methods
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Execute(Options.SendGridKey, subject, message, email);
        }
        #endregion

        #region Utils
        private Task Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(Options.SendGridEmail, Options.SendGridName),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);

            return client.SendEmailAsync(msg);
        }
        #endregion

    }
}
