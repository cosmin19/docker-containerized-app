using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Enviroself.Services.EmailSender
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
