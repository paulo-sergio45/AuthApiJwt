using AuthApi.Entities;
using AuthApi.Interface;
using AuthApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers.v1
{
    [ApiController]
    [Route("v1/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;

        public AuthController(ILogger<AuthController> logger, UserManager<ApplicationUser> userManager, ITokenService tokenService)
        {
            _logger = logger;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);


            if (!result.Succeeded)
                return BadRequest("Erro ao registrar usuário.");

            await _userManager.AddToRoleAsync(user, "user");
            return Ok("Usuário registrado.");

        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateTokenAcces(user, roles);



            return Ok(new { message = "Login efetuado com sucesso!", token });

        }
    }
}
