using LeonardoStore.Customer.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeonardoStore.Customer.Infra.Mappings
{
    public class AddressMapping : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.Id);
            
            builder.Property(c => c.City)
                .IsRequired()
                .HasColumnType("varchar(80)");
            
            builder.Property(c => c.State)
                .IsRequired()
                .HasColumnType("varchar(20)");
            
            builder.Property(c => c.Country)
                .IsRequired()
                .HasColumnType("varchar(80)");
            
            builder.Property(c => c.Neighborhood)
                .IsRequired()
                .HasColumnType("varchar(80)");
            
            builder.Property(c => c.Number)
                .IsRequired()
                .HasColumnType("varchar(30)");
            
            builder.Property(c => c.Street)
                .IsRequired()
                .HasColumnType("varchar(80)");
            
            builder.Property(c => c.ZipCode)
                .IsRequired()
                .HasColumnType("varchar(9)");

            builder.ToTable("address");
        }
    }
}