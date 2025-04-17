using System.Text.Json.Serialization;

namespace dotnet_api.Shared.DTOs;
public class CategoriaDTO
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? Descricao { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ICollection<ProdutoDTO>? Produtos { get; set; } = [];

}
