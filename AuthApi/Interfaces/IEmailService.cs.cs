
using AuthApi.Models;

namespace AuthApi.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailModel emailData);
    }
}