using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models.Dtos
{
    public record RegisterDto(
        [Required(ErrorMessage = "O nome é obrigatório.")]
        string Name,

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
        string Email,

        string PhoneNumber,

        [Required(ErrorMessage = "A senha é obrigatória.")]
        string Password);
}
