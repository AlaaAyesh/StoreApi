
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreApis.Helper;
using StoreCore;
using StoreCore.Mapping.Products;
using StoreCore.ServicesContract;
using StoreRepository;
using StoreRepository.Data;
using StoreRepository.Data.Contexts;
using StoreService.Service.Products;

namespace StoreApis
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // read allowed origins from configuration
            var allowedOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? new string[0];

            // register CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DefaultCorsPolicy", policy =>
                {
                    policy
                        .WithOrigins(allowedOrigins)      // origins „‰ appsettings
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();             // ·Ê  ” Œœ„ Authentication cookies √Ê credentials
                                                         // .SetIsOriginAllowedToAllowWildcardSubdomains(); // «Œ Ì«—Ì ··‹ wildcard subdomains
                });
            });


            builder.Services.AddDependencyServices(builder.Configuration);

            var app = builder.Build();
            app.UseCors("DefaultCorsPolicy");


            await app.UseConfigureMiddelware();

            app.Run();
            
        }
    }
}
