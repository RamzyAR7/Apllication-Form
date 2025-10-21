using Application_Form.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application_Form.Infrastructure.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
           builder.ToTable("Clients");
           builder.HasKey(c => c.Id);
           builder.Property(c => c.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("bigint");
           builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        }
    }
}
