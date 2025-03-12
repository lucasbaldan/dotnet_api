using dotnet_api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dotnet_api.Database.TableConfigurations
{
    public class CategoriaConfiguration : IEntityTypeConfiguration<Categoria>
    {

        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.ToTable("CATEGORIAS");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Nome).IsRequired().HasMaxLength(255);
            builder.Property(c => c.Descricao).HasMaxLength(255);
        }
    }
}
