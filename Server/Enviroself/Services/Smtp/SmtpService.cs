using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Enviroself.Services.Smtp
{
    public class SmtpService : ISmtpService
    {
        #region Fields
        private readonly IConfiguration _configuration;
        #endregion

        #region Ctor
        public SmtpService(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        #endregion

        #region Methods
        public virtual async Task SendEmail(string toAddress, string body, string subject)
        {
            string fromAddress = _configuration["EmailAccount:Email"];

            var client = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress, _configuration["EmailAccount:Password"])
            };
            client.SendCompleted += (s, e) => client.Dispose();


            MailMessage mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(fromAddress);
            mailMessage.To.Add(toAddress);
            mailMessage.Body = body;
            mailMessage.Subject = subject;

            await client.SendMailAsync(mailMessage);
        }
        #endregion

    }
}
