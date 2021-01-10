using CursoEFCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CursoEFCore.Data.Configurations
{
    public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("Clientes");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Nome).HasColumnType("Varchar(80)").IsRequired();
            builder.Property(p => p.Telefone).HasColumnType("char(11)");
            builder.Property(p => p.CEP).HasColumnType("char(8)").IsRequired();
            builder.Property(p => p.Estado).HasColumnType("char(2)").IsRequired();
            builder.Property(p => p.Cidade).HasMaxLength(60).IsRequired();
            builder.Property(p => p.Nome).HasColumnType("Varchar(80)").IsRequired();

            builder.HasIndex(i => i.Telefone).HasName("idx_cliente_telefone");
        }
    }
}