using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using StoreCore.Dtos.Basket;

namespace StoreCore.Mapping.Basket
{
    public class BasketProfile : Profile
    {
        public BasketProfile(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            // CreateMap<Source, Destination>();
            CreateMap<Entities.Basket, BasketDto>().ReverseMap();
            CreateMap<Entities.CustomerBasket, CustomerBasketDto>().ReverseMap();
        }
    }
}
