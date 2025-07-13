
using System.Net;
using AuthApi.Exceptions;


namespace AuthApi.Middlewares
{
    public class ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Ocorreu um erro inesperado");
                await HandleExceptionAsync(httpContext, exception);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            List<(string Campo, string Mensagem)> errors = new();

            switch (exception)
            {

                case UnauthorizedException exceptionCase:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errors.Add(("messagen", exceptionCase.Message));
                    break;

                case NotFoundException exceptionCase:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    errors.Add(("messagen", exceptionCase.Message));
                    break;

                case BadRequestException exceptionCase:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errors.Add(("messagen", exceptionCase.Message));
                    break;

                case SendEmailException exceptionCase:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errors.Add(("messagen", exceptionCase.Message));
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errors.Add(("messagen", "Ocorreu um erro inesperado. Tente novamente mais tarde."));
                    break;
            }

            var userFriendlyResponse = new
            {
                Type = $"https://tools.ietf.org/html/rfc9110#section-15.{context.Response.StatusCode}",
                Title = "An error occured",
                Status = context.Response.StatusCode,
                errors = errors
               .GroupBy(e => e.Campo)
               .ToDictionary(
                   g => g.Key,
                   g => g.Select(e => e.Mensagem).ToArray()
               ),
                Detail = exception.Message,
            };

            return context.Response.WriteAsJsonAsync(userFriendlyResponse);
        }
    }
}
