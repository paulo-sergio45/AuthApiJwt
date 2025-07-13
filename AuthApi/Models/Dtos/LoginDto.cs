using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models.Dtos
{
    public record LoginDto(
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
        string Email,

        [Required(ErrorMessage = "A senha é obrigatória.")]
        string Password);
}
