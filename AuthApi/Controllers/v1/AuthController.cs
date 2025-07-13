using AuthApi.Entities;
using AuthApi.Exceptions;
using AuthApi.Interfaces;
using AuthApi.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers.v1
{
    [ApiController]
    [Route("v1/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {

        private readonly IAuthService _authService = authService;

        [HttpPost]
        [Route("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {

            string resultado = await _authService.Register(model);

            return Ok(resultado);

        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            TokenDto resultado = await _authService.Login(model);

            return Ok(resultado);

        }

        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailDto model)
        {
            string resultado = await _authService.ConfirmEmail(model);

            return Ok(resultado);

        }

        [HttpGet("send-confirmation-email")]
        public async Task<IActionResult> SendEmailConfirmation([FromQuery] string email)
        {
            string resultado = await _authService.SendEmailConfirmation(email);

            return Ok(resultado);
        }
    }
}
