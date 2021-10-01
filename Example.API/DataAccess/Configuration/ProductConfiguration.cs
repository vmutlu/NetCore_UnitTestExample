using Example.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Example.API.DataAccess.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> modelBuilder)
        {
            modelBuilder.Property(e => e.Id);

            modelBuilder.Property(e => e.Color).HasMaxLength(50);

            modelBuilder.Property(e => e.Name).HasMaxLength(100);

            modelBuilder.Property(e => e.Price);
        }
    }
}
