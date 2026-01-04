using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using StoreCore.Dtos.Orders;
using StoreCore.Entities.Order;

namespace StoreCore.Mapping.Orders
{
    public class OrderProfile:Profile
    {
        public OrderProfile(IConfiguration configuration)
        {
            CreateMap<Order, OrderToReturnDto>()
                .ForMember(p => p.DeliveryMethod, o => o.MapFrom(p => p.DeliveryMethod.ShortName))
                .ForMember(p => p.DeliveryMethodPrice, o => o.MapFrom(p => p.DeliveryMethod.Price))
                .ForMember(p => p.Subtotal,o => o.MapFrom(p => p.Subtotal))
                ;

            CreateMap<OrderAddress, AddressOrderDto>().ReverseMap();


            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(p => p.ProductId, o => o.MapFrom(p => p.Product.ProductItemId))
                .ForMember(p => p.ProductName, o => o.MapFrom(p => p.Product.ProductName))
                .ForMember(p => p.PictureUrl, o => o.MapFrom(p => $"{configuration["BaseURL"]}{p.Product.PictureUrl}"));
        }
    }
}
