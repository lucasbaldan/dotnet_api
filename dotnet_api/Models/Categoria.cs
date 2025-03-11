using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dotnet_api.Models;

[Table("Categorias")]
public class Categoria
{
    public Categoria()
    {
        Produtos = new Collection<Produto>();
    }
    
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string? Nome { get; set; }

    [StringLength(500)]
    public string? Descricao { get; set; }

    public ICollection<Produto>? Produtos { get; set; }

}
