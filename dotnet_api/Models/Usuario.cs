using Microsoft.AspNetCore.Identity;

namespace dotnet_api.Models;

public class Usuario : IdentityUser
{
    public string? RefreshToken { get; set; }

    public string? RefreshTokenExpiryTime { get; set; }
}
