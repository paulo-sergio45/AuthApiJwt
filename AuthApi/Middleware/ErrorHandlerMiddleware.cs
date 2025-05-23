using System.Net;
using Microsoft.AspNetCore.Http;

namespace AuthApi.Middleware
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            _logger.LogError(exception, "Ocorreu um erro inesperado");

            var userFriendlyResponse = new { message = "Ocorreu um erro inesperado. Tente novamente mais tarde." };

            switch (exception)
            {
                case ArgumentException argEx:
                    context.Response.StatusCode  = (int) HttpStatusCode.BadRequest;
                    userFriendlyResponse = new { message = "Houve um erro nos parâmetros fornecidos." };
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    userFriendlyResponse = new { message = "Você não tem permissão para acessar este recurso." };
                    break;

                case KeyNotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    userFriendlyResponse = new { message = "O recurso solicitado não foi encontrado." };
                    break;

                case InvalidOperationException:
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    userFriendlyResponse = new { message = "A operação não pode ser concluída no momento." };
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            return context.Response.WriteAsJsonAsync(userFriendlyResponse);
        }
    }
}
