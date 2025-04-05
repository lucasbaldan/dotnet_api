using dotnet_api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dotnet_api.Database.TableConfigurations
{
    public class PessoaConfiguration : IEntityTypeConfiguration<Pessoa>
    {

        public void Configure(EntityTypeBuilder<Pessoa> builder)
        {
            builder.ToTable("pessoas");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Nome).IsRequired().HasMaxLength(500);
            builder.Property(p => p.Telefone).HasMaxLength(50);
            builder.Property(p => p.Estado).HasMaxLength(255);
            builder.Property(p => p.Email).HasMaxLength(500);
            builder.Property(p => p.Logradouro).HasMaxLength(1200);
            builder.Property(p => p.Cidade).HasMaxLength(255);
            builder.Property(p => p.Cep).HasMaxLength(10);

        }
    }
}
