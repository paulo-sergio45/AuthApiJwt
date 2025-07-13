using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models.Dtos
{
    public record UserDto(

        [Required(ErrorMessage = "O id é obrigatório.")]
        string id,

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
        string Email,

        [Required(ErrorMessage = "O nome é obrigatório.")]
        string username);


}
