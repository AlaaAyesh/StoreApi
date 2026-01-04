using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreCore.Entities.Order;

namespace StoreRepository.Data.Configurations
{
    public class OrderItemConfigurations : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.OwnsOne(i => i.Product, io =>
            {
                io.WithOwner();
   
            });
            builder.Property(i => i.Price).HasColumnType("decimal(18,2)");
        }
    
    
    }
}
