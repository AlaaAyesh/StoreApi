using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using StoreCore.Dtos.Products;
using StoreCore.Entities;

namespace StoreCore.Mapping.Products
{
    public class ProductProfile : Profile
    {
        public ProductProfile(IConfiguration configuration)
        {
            CreateMap<Product, ProductDto>().ForMember(d => d.Brand, opt => opt.MapFrom(s => s.Brand!.Name))
                                            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.Type!.Name))
                                            //.ForMember(d => d.PictureUrl, opt => opt.MapFrom(s => $"{configuration["BaseURL"]}{s.PictureUrl}" ?? "default-image.png"))
                                            .ForMember(d => d.PictureUrl, opt => opt.MapFrom(new PictureUrlResolver(configuration)))
                                            ;            
            CreateMap<ProductBrand, TypeBrandDto>();
            CreateMap<ProductType, TypeBrandDto>();
        }
    }
}
