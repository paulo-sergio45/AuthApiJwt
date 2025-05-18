using System.Security.Claims;

namespace AuthApi.Endpoints
{
    public class UserEndpoints
    {
        public static void ConfigureEndpoints(WebApplication app)
        {
            app.MapGet("/protected", (ClaimsPrincipal user) =>
            {
                return Results.Ok($"Olá {user.Identity?.Name}, você está autenticado!");
            })
            .RequireAuthorization();  // Protege o endpoint
        }
    }
}
