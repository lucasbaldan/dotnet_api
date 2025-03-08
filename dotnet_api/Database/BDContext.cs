using dotnet_api.Models;
using Microsoft.EntityFrameworkCore;

namespace dotnet_api.Database;
public class BDContext : DbContext
{
    public BDContext(DbContextOptions<BDContext> options) : base(options)
    {
    }

    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Produto> Produtos { get; set; }
}
