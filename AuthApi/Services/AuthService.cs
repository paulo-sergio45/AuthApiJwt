using AuthApi.Entities;
using AuthApi.Exceptions;
using AuthApi.Interfaces;
using AuthApi.Models;
using AuthApi.Models.Dtos;
using AuthApiCoreIdentity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthApi.Services
{

    public class AuthService(UserManager<User> userManager, ITokenService tokenService, IEmailService emailService, IOptions<AppSettings> appSettings) : IAuthService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IEmailService _emailService = emailService;
        private readonly AppSettings _appSettings = appSettings.Value;

        public async Task<string> Register(RegisterDto model)
        {


            var user = new User { UserName = model.Name, Email = model.Email, PhoneNumber = model.PhoneNumber };
            var result = await _userManager.CreateAsync(user, model.Password);


            if (!result.Succeeded)
                throw new Exception("Erro ao registrar usuário.");

            await SendEmailConfirmation(user.Email);

            await _userManager.AddToRoleAsync(user, "User");
            return "Usuário registrado.";
        }

        public async Task<TokenDto> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                throw new UnauthorizedException("Nao permitido");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateTokenAccess(user, roles);

            UserDto userDto = new UserDto(user.Id, user.Email, user.UserName);

            return new TokenDto("Login efetuado com sucesso!", token, userDto);

        }

        public async Task<string> ConfirmEmail(ConfirmEmailDto model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                throw new NotFoundException("Usuário não encontrado.");

            var result = await _userManager.ConfirmEmailAsync(user, model.Token);
            if (result.Succeeded)
            {

                EmailModel emailModel = new EmailModel
                {
                    ToAddress = [user.Email],
                    Subject = "Boas Vindas a API",
                    Template = "BoasVindas",
                    TemplateModel = new BoasVindasModel(user.UserName)
                };
                await _emailService.SendEmailAsync(emailModel);

                return "E-mail confirmado com sucesso.";
            }

            throw new BadRequestException("Falha na confirmação do e-mail.");
        }

        public async Task<string> SendEmailConfirmation(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || user.EmailConfirmed)
                throw new BadRequestException("Usuário inválido ou e-mail já confirmado.");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink = $"{_appSettings.FrontendUrl}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";


            EmailModel emailModel = new EmailModel
            {
                ToAddress = [user.Email],
                Subject = "Confirmação de E-mail",
                Template = "ConfirmEmail",
                TemplateModel = new ConfirmEmailModel(user.UserName, confirmationLink)
            };


            await _emailService.SendEmailAsync(emailModel);

            return "Link de confirmação enviado.";
        }
    }
}
