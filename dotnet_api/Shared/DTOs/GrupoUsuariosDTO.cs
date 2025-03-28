using System.ComponentModel.DataAnnotations;

namespace dotnet_api.Shared.DTOs;

public class GrupoUsuariosDTO
{
    public string? Id { get; set; }

    [Required(ErrorMessage = "O campo --nome-- é obrigatório.")]
    public string? Nome { get; set; }

    [Required(ErrorMessage = "O campo --ativo-- é obrigatório.")]
    public bool? Ativo { get; set; }

    public string? Descricao { get; set; }

    public List<int>? Permissoes { get; set; }

}
