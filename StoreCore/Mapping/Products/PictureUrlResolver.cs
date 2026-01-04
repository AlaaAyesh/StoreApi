using System;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using StoreCore.Dtos.Products;
using StoreCore.Entities;

namespace StoreCore.Mapping.Products
{
    public class PictureUrlResolver : IValueResolver<Product, ProductDto, string>
    { 
        private readonly IConfiguration  _configuration;
        public PictureUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string Resolve(Product source, ProductDto destination, string destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.PictureUrl))
            {
                return null;
            }
            var baseUrl = _configuration["BaseURL"];
            return $"{baseUrl}{source.PictureUrl}";
        } 

    }
}
