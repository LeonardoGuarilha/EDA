using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeonardoStore.Customer.Infra.Mappings
{
    public class CustomerMapping : IEntityTypeConfiguration<Domain.Entities.Customer>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Customer> builder)
        {
            builder.HasKey(c => c.Id);

            builder.OwnsOne(c => c.Name, cm =>
            {
                cm.Property(c => c.FirstName)
                    .HasColumnName("first_name")
                    .HasColumnType("varchar(40)");
                
                cm.Property(c => c.LastName)
                    .HasColumnName("last_name")
                    .HasColumnType("varchar(40)");
            });

            builder.OwnsOne(c => c.Document, cm =>
            {
                cm.Property(c => c.Number)
                    .HasColumnName("document")
                    .HasColumnType("varchar(14)");
                
                cm.Property(c => c.Type)
                    .HasColumnName("document_type")
                    .HasColumnType("int");
            });

            builder.OwnsOne(c => c.Email, cm =>
            {
                cm.Property(c => c.Address)
                    .HasColumnName("email")
                    .HasColumnType("varchar(150)");
            });

            builder.HasMany(c => c.Addresses)
                .WithOne(c => c.Customer)
                .HasForeignKey(c => c.CustomerId);

            builder.Property(c => c.CreatedAt);
            builder.Property(c => c.LastUpdateDate);

            builder.ToTable("customers");

        }
    }
}