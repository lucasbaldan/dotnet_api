using dotnet_api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using dotnet_api.Database.TableConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace dotnet_api.Database;
public class BDContext(DbContextOptions<BDContext> options) : IdentityDbContext<Usuario, GrupoUsuarios, string>(options)
{
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Pessoa> Pessoas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProdutoConfiguration());
        modelBuilder.ApplyConfiguration(new CategoriaConfiguration());
        modelBuilder.ApplyConfiguration(new PessoaConfiguration());

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>().ToTable("usuarios");
        modelBuilder.Entity<GrupoUsuarios>().ToTable("grupos_usuarios");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("usuarios_grupo_usuarios");
    }
}
