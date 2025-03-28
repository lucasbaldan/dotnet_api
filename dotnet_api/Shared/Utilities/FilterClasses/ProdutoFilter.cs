namespace dotnet_api.Shared.Utilities.FilterClasses;

public class ProdutoFilter
{
    public int? Id { get; set; }
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public decimal? MinPreco { get; set; }
    public decimal? MaxPreco { get; set; }
    public int? MinEstoque { get; set; }
    public int? MaxEstoque { get; set; }
    public int? CategoriaId { get; set; }

}
