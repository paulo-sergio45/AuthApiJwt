using AuthApi.Models.Dtos;

namespace AuthApi.Interfaces
{

    public interface IAuthService
    {
        Task<string> Register(RegisterDto model);
        Task<TokenDto> Login(LoginDto model);
        Task<string> ConfirmEmail(ConfirmEmailDto model);
        Task<string> SendEmailConfirmation(string email);
    }
}


