using System.Threading.Tasks;

namespace Enviroself.Services.Smtp
{
    public interface ISmtpService
    {
        Task SendEmail(string toAddress, string body, string subject);
    }
}
