using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using StoreRepository.Data.Contexts;

namespace StoreRepository.Data
{
    public static class StoreDbContextSeed
    {
        public static async Task SeedAsync(StoreDbContext context)
        {
            var basePath = @"..\StoreRepository\Data\DataSeed\";

            try
            {
                if (context.Brands.Count() == 0)
                {
                    var brandsData = File.ReadAllText(Path.Combine(basePath, "brands.json"));
                    var brands = JsonSerializer.Deserialize<List<StoreCore.Entities.ProductBrand>>(brandsData);

                    if (brands?.Any() == true)
                    {
                        await context.Brands.AddRangeAsync(brands);
                        await context.SaveChangesAsync();
                    }
                }

                if (context.Types.Count() == 0)
                {
                    var typesData = File.ReadAllText(Path.Combine(basePath, "types.json"));
                    var types = JsonSerializer.Deserialize<List<StoreCore.Entities.ProductType>>(typesData);

                    if (types?.Any() == true)
                    {
                        await context.Types.AddRangeAsync(types);
                        await context.SaveChangesAsync();
                    }
                }

                if (context.Poducts.Count() == 0)
                {
                    var productsData = File.ReadAllText(Path.Combine(basePath, "products.json"));
                    var products = JsonSerializer.Deserialize<List<StoreCore.Entities.Product>>(productsData);

                    if (products?.Any() == true)
                    {
                        
                       
                        await context.Poducts.AddRangeAsync(products);
                        await context.SaveChangesAsync();
                    }
                }
                if (context.DeliveryMethods.Count() == 0)
                {
                    var dmData = File.ReadAllText(Path.Combine(basePath, "delivery.json"));
                    var methods = JsonSerializer.Deserialize<List<StoreCore.Entities.Order.DeliveryMethod>>(dmData);
                    if (methods?.Any() == true)
                    {
                        await context.DeliveryMethods.AddRangeAsync(methods);
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error seeding data: {ex.Message}");
            }
        }
    }


}
