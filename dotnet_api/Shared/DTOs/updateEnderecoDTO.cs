using System.ComponentModel.DataAnnotations;

namespace dotnet_api.Shared.DTOs;

public class UpdateEnderecoDTO
{
    [Required(ErrorMessage = "O campo --id_pessoa-- é obrigatório.")]
    public int? Id_pessoa { get; set; }

    [Required(ErrorMessage = "O campo --cep-- é obrigatório.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "O campo --cep-- deve conter exatamente 8 dígitos.")]
    public string? Cep { get; set; }

}
