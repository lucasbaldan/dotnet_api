using dotnet_api.Database.TableConfigurations;
using dotnet_api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace dotnet_api.Database;
public class BDContext : IdentityDbContext<Usuario, GrupoUsuarios, string>
{
    public BDContext(DbContextOptions<BDContext> options) : base(options)
    {
    }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Produto> Produtos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProdutoConfiguration());
        modelBuilder.ApplyConfiguration(new CategoriaConfiguration());
        
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>().ToTable("usuarios");
        modelBuilder.Entity<GrupoUsuarios>().ToTable("grupos_usuarios");
    }
}
