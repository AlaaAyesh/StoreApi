using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreCore.Entities.Order;

namespace StoreRepository.Data.Configurations
{
    public class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.Subtotal).HasColumnType("decimal(18,2)");
            builder.Property(o =>o.Status).HasConversion(OStatus=>OStatus.ToString(),OStatus=>(OrderStatus)Enum.Parse(typeof(OrderStatus),OStatus));
            builder.OwnsOne(o => o.ShipToAddress, a =>
            {
                a.WithOwner();
        
            });

            builder.HasOne (o => o.DeliveryMethod).WithMany().HasForeignKey(o => o.DeliveryMethodId);        }
    }
}
