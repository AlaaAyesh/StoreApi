using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreCore.Entities;

namespace StoreRepository.Data.Configurations
{
    public class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
            builder.Property(p => p.Price).HasColumnType("decimal(10,2)");
            builder.Property(p => p.PictureUrl).IsRequired();
            builder.HasOne(p => p.Brand)
                   .WithMany()
                   .HasForeignKey(p => p.BrandId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(p => p.Type).WithMany()
                   .HasForeignKey(p => p.TypeId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.Property(p=>p.BrandId).IsRequired(false); // Assuming BrandId is optional
            builder.Property(p => p.TypeId).IsRequired(false); // Assuming TypeId is optional


            builder.Property(p => p.Description)
                   .HasMaxLength(500)
                   .IsRequired(false); // Assuming Description is optional




        }
    }
   
}
