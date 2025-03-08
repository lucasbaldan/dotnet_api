namespace dotnet_api.Models;

public class Produto
{
    public int Id { get; set; }

    public required string Nome { get; set; }

    public string? Descricao { get; set; }

    public decimal Preco { get; set; }

    public float Estoque { get; set; }

    public DateTime DataCadastro { get; set; }

    public int CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }

}
