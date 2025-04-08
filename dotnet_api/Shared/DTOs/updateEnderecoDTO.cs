using System.ComponentModel.DataAnnotations;

namespace dotnet_api.Shared.DTOs;

public class UpdateEnderecoDTO
{
    [Required(ErrorMessage = "O campo --id_pessoa-- é obrigatório.")]
    public int? Id_pessoa { get; set; }

    [Required(ErrorMessage = "O campo --cep-- é obrigatório.")]
    public int? Cep { get; set; }

}
