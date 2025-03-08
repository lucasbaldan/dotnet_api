using System.Collections.ObjectModel;

namespace dotnet_api.Models;
public class Categoria
{
    public Categoria()
    {
        Produtos = new Collection<Produto>();
    }
    public int Id { get; set; }
    public required string Nome { get; set; }
    public string? Descricao { get; set; }

    public ICollection<Produto>? Produtos { get; set; }

}
