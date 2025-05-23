using AuthApi.Entities;
using AuthApi.Interface;
using AuthApi.Models;
using AuthApi.Services;
using Microsoft.AspNetCore.Identity;

namespace AuthApi.Endpoints
{
    public class AuthEndpoints
    {
        public static void ConfigureEndpoints(WebApplication app)
        {
            app.MapPost("/register", async (UserManager<ApplicationUser> _userManager, RegisterModel model) =>
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);


                if (!result.Succeeded)
                    return Results.BadRequest("Erro ao registrar usuário.");

                await _userManager.AddToRoleAsync(user, "user");
                return Results.Ok("Usuário registrado.");


            })
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

            app.MapPost("/login", async (UserManager<ApplicationUser> userManager, ITokenService jwtService, LoginModel model, HttpContext http) =>
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
                    return Results.Unauthorized();

                var roles = await userManager.GetRolesAsync(user);
                var token = jwtService.GenerateTokenAcces(user, roles);



                return Results.Ok(new { message = "Login efetuado com sucesso!", token });
            }).AllowAnonymous();

            app.MapPost("/logout", (HttpContext http) =>
            {

                return Results.Ok("Deslogado com sucesso.");
            }).RequireAuthorization();
        }
    }
}
