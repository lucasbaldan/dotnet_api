using dotnet_api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dotnet_api.Database.TableConfigurations
{
    public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
    {

        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.ToTable("produtos");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Nome).IsRequired().HasMaxLength(255);
            builder.Property(p => p.Descricao).HasMaxLength(255);
            builder.Property(p => p.Preco).HasColumnType("decimal(10,2)").IsRequired();
            builder.Property(p => p.Estoque).IsRequired();
            builder.Property(p => p.DataCadastro).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(p => p.CategoriaId).IsRequired();
            builder.HasOne(p => p.Categoria).WithMany(c => c.Produtos).HasForeignKey(p => p.CategoriaId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
