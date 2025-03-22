using System.ComponentModel.DataAnnotations;

namespace dotnet_api.DTOs;

public class RefreshTokenDTO
{
    [Required]
    public required string AcessToken { get; set; }

    [Required]
    public required string RefreshToken { get; set; }
}
