using AuthApi.Models;
using AuthApi.Services;
using Microsoft.AspNetCore.Identity;

namespace AuthApi.Endpoints
{
    public class AuthEndpoints
    {
        public static void ConfigureEndpoints(WebApplication app)
        {
            app.MapPost("/register", async (UserManager<ApplicationUser> userManager, RegisterModel model) =>
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);
                return result.Succeeded ? Results.Ok("Usuário criado com sucesso!") : Results.BadRequest(result.Errors);
            })
            .RequireAuthorization(policy => policy.RequireRole("Admin"));

            app.MapPost("/login", async (UserManager<ApplicationUser> userManager, JwtService jwtService, LoginModel model, HttpContext http) =>
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
                    return Results.Unauthorized();

                var roles = await userManager.GetRolesAsync(user);
                var token = jwtService.GenerateToken(user, roles);

                http.Response.Cookies.Append("access_token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(60)
                });

                return Results.Ok(new { Message = "Login efetuado com sucesso!" });
            });

            app.MapPost("/logout", (HttpContext http) =>
            {
                http.Response.Cookies.Delete("access_token");
                return Results.Ok("Deslogado com sucesso.");
            });
        }
    }
}
