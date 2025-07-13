namespace AuthApi.Exceptions
{
    public class UnauthorizedException(string message) : Exception(message)
    {
    }

    public class NotFoundException(string message) : Exception(message)
    {
    }

    public class ArgumentsException(string message) : Exception(message)
    {
    }

    public class BadRequestException(string message) : Exception(message)
    {
    }
    public class SendEmailException(string message) : Exception(message)
    {
    }
}
