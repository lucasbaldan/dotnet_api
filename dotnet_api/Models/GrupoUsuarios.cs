using Microsoft.AspNetCore.Identity;

namespace dotnet_api.Models;

public class GrupoUsuarios : IdentityRole
{
    public bool? Ativo { get; set; }

    public string? Descricao { get; set; }

    public string? PermissoesJson { get; set; }
}
