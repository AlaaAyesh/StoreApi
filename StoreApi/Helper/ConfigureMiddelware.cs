using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoreRepository.Data;
using StoreRepository.Data.Contexts;
using StoreRepository.Identity;
using StoreRepository.Identity.Contexts;

namespace StoreApis.Helper
{
    public static class ConfigureMiddelware
    {
        public static async Task<WebApplication> UseConfigureMiddelware(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var sevices = scope.ServiceProvider;

            var context = sevices.GetRequiredService<StoreDbContext>();// ask for the db context from the service container
            var identityContext = sevices.GetRequiredService<StoreIdentityDbContext>();// ask for the db context from the service container
            var loggerFactory = sevices.GetRequiredService<ILoggerFactory>();
            try
            {
                await context.Database.MigrateAsync(); // Apply any pending migrations in database
                StoreDbContextSeed.SeedAsync(context).Wait(); // Seed the database with initial data
                await identityContext.Database.MigrateAsync();
                await StoreIdenityDbContextSeed.SeedAppUserAsync(sevices.GetRequiredService<UserManager<StoreCore.Entities.Identity.AppUser>>());
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred during migration");
            }

            app.UseMiddleware<StoreApis.Middlewares.ExceptionMiddleware>();



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }


            // 
            app.UseStatusCodePagesWithReExecute("/api/errors/{0}"); // Handle status code pages 

            app.UseStaticFiles();

            app.UseHttpsRedirection();


            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();


           return app;
        }
    }
}
