namespace AuthApi.Models.Dtos
{

    public record TokenDto(string Message, string Token,UserDto User);
}
