using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_api.Models;
public class Categoria
{
    public Categoria()
    {
        Produtos = new Collection<Produto>();
    }
   
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? Descricao { get; set; }
    public ICollection<Produto>? Produtos { get; set; }

}
