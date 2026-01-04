using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using StoreCore;
using StoreCore.Entities.Identity;
using StoreCore.Mapping.Auth;
using StoreCore.Mapping.Basket;
using StoreCore.Mapping.Orders;
using StoreCore.Mapping.Products;
using StoreCore.RepositoriesContract;
using StoreCore.ServicesContract;
using StoreRepository;
using StoreRepository.Data.Contexts;
using StoreRepository.Identity.Contexts;
using StoreRepository.Repositoies;
using StoreService.Service.Baskets;
using StoreService.Service.Caches;
using StoreService.Service.Orders;
using StoreService.Service.Payment;
using StoreService.Service.Payment.Strategies;
using StoreService.Service.Products;
using StoreService.Service.Tokens;
using StoreService.Service.Users;

namespace StoreApis.Helper
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddBuiltInServices();
            services.AddSwagerServices();
            services.AddDbContextServices(configuration);

            // سجل Identity قبل أي service يعتمد عليه
            services.AddIdentityService(configuration);

            // بعد تسجيل Identity، سجل باقي الخدمات التي قد تعتمد على SignInManager/UserManager
            services.AddUserDefinedServices();

            services.AddAutoMapperServices(configuration);
            services.AddConfigureServices();
            services.AddRedisService(configuration);


            services.AddAuthenticationService(configuration);

            return services;
        }

        private static IServiceCollection AddBuiltInServices(this IServiceCollection services)
        {
            services.AddControllers();
            return services;
        }

        private static IServiceCollection AddSwagerServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }

        private static IServiceCollection AddDbContextServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<StoreDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure()));

            services.AddDbContext<StoreIdentityDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultIdentityConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure()));

            return services;
        }

        private static IServiceCollection AddUserDefinedServices(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUnitOfWork, UnitOfWorkcs>();
            services.AddScoped<IBasketRepository, BasketRepository>();

            // UserService يعتمد على SignInManager / UserManager
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ITokenService, TokenService>();
           
            services.AddScoped<PaymobClient>();
            services.AddScoped<IPaymentService, PaymentService>();
            // --- Payment setup ---
            services.AddScoped<PaymobCardPaymentStrategy>();
            services.AddScoped<PaymobWalletPaymentStrategy>();
            services.AddScoped<CashPaymentStrategy>();

            services.AddScoped<IPaymentStrategyFactory, PaymentStrategyFactory>();
            services.AddScoped<IPaymentService, PaymentService>();



            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IBasketService, BasketService>();


            return services;
        }

        private static IServiceCollection AddAutoMapperServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(M => M.AddProfile(new ProductProfile(configuration)));
            services.AddAutoMapper(M => M.AddProfile(new BasketProfile(configuration)));
            services.AddAutoMapper(M => M.AddProfile(new OrderProfile(configuration)));
            services.AddAutoMapper(M => M.AddProfile(new AuthProfile()));
            return services;
        }

        private static IServiceCollection AddConfigureServices(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage).ToArray();

                    var errorResponse = new Errors.ApiValidationErrorResponse
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(errorResponse);
                };
            });

            return services;
        }

       
private static IServiceCollection AddRedisService(this IServiceCollection services, IConfiguration configuration)
    {
        // Read from either section style or ConnectionStrings
        var redisConn = configuration["Redis:ConnectionString"] ?? configuration.GetConnectionString("Redis");
        if (string.IsNullOrWhiteSpace(redisConn))
        {
            // إنك تختاري هل تريدين رمي خطأ لو مش لاقي الكونكشن، أو تسجلين تحذير فقط
            throw new InvalidOperationException("Redis connection string not found. Add Redis:ConnectionString or ConnectionStrings:Redis in appsettings.");
        }

        // Parse URI (supports redis:// and rediss://)
        var uri = new Uri(redisConn);

        var options = new ConfigurationOptions
        {
            AbortOnConnectFail = false, // مهم: لا تفشل نهائياً على أول محاولة
            Ssl = uri.Scheme.Equals("rediss", StringComparison.OrdinalIgnoreCase),
            KeepAlive = 180,
            ConnectTimeout = 5000,
            SyncTimeout = 5000
        };

        options.EndPoints.Add(uri.Host, uri.Port);

        if (!string.IsNullOrEmpty(uri.UserInfo))
        {
            var parts = uri.UserInfo.Split(':', 2);
            if (parts.Length == 2)
            {
                options.User = parts[0];
                options.Password = parts[1];
            }
            else
            {
                options.Password = parts[0];
            }
        }

        // Optional: set default flags for managed providers
        options.AllowAdmin = false;

        // Register IConnectionMultiplexer as Singleton
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var logger = sp.GetService<ILoggerFactory>()?.CreateLogger("Redis");
            try
            {
                // Attempt async connect and wait synchronously (so startup can fail explicitly if desired)
                logger?.LogInformation("Attempting to connect to Redis at {Host}:{Port} (Ssl={Ssl})", uri.Host, uri.Port, options.Ssl);
                var mux = ConnectionMultiplexer.ConnectAsync(options).GetAwaiter().GetResult();
                logger?.LogInformation("Connected to Redis successfully.");
                return mux;
            }
            catch (Exception ex)
            {
                // Log the error, but because AbortOnConnectFail=false the multiplexer will retry if created.
                logger?.LogWarning(ex, "Initial Redis connection failed. Falling back to non-blocking connection (multiplexer will retry).");

                // Fallback: create a multiplexer synchronously with same options.
                // With AbortOnConnectFail = false it should not throw permanently and will retry in background.
                try
                {
                    var muxFallback = ConnectionMultiplexer.Connect(options);
                    logger?.LogInformation("Fallback multiplexer created (will retry in background).");
                    return muxFallback;
                }
                catch (Exception fallbackEx)
                {
                    // If even fallback fails, rethrow or return null depending on desired behavior.
                    logger?.LogError(fallbackEx, "Fallback Redis connection also failed.");
                    throw; // أو: return null; لكن إذا رجعنا null فـ GetRequiredService سيكسر أماكن الاستخدام.
                }
            }
        });

        return services;
    }


    private static IServiceCollection AddIdentityService(this IServiceCollection services, IConfiguration configuration)
        {
            // استخدم AddIdentity لاضافة SignInManager, UserManager, RoleManager, token providers, ...
            services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 3;
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<StoreIdentityDbContext>()
            .AddDefaultTokenProviders();

          
            return services;
        }
        private static IServiceCollection AddAuthenticationService(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(
                options=>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["jwt:Key"])),
                        ValidateIssuer = true,
                        ValidIssuer = configuration["jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["jwt:Audience"],
                    };
                });
            services.AddAuthorization();



            return services;
        }
        



    }
}
