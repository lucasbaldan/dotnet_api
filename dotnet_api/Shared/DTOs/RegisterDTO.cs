using System.ComponentModel.DataAnnotations;

namespace dotnet_api.Shared.DTOs;

public class RegisterDTO
{
    public string? Id { get; set; }

    [Required]
    public required string Username { get; set; }

    [Required]
    public required string Password { get; set; }

    [Required]
    public required string Email { get; set; }

    [Required]
    public required string GrupoUsuariosID { get; set; }
}
