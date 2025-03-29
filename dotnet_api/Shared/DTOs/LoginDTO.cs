using System.ComponentModel.DataAnnotations;

namespace dotnet_api.Shared.DTOs;

public class LoginDTO
{
    [Required(ErrorMessage = "O campo --email-- é obrigatório.")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "O campo --senha-- é obrigatório.")]
    [MinLength(4, ErrorMessage = "A --email-- deve ter pelo menos 4 caracteres.")]
    public required string Password { get; set; }
}
