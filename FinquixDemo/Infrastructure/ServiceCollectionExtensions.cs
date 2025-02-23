﻿using FinquixDemo.Infrastructure.Database;
using FinquixDemo.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace FinquixDemo.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FinquixDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("FinquixDemo"));
            });

            services.AddHttpContextAccessor();
            services.AddHostedService<MarketDataSimulatorService>();
            //services.AddRepositories();
            //services.AddStorageManager(configuration);
            //services.AddServices();

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            return services;
        }

        public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();

            if (allowedOrigins == null || allowedOrigins.Length == 0)
            {
                throw new InvalidOperationException("CORS configuration is missing or empty in appsettings.json.");
            }

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .SetIsOriginAllowed(origin => true) // ✅ Ensures local requests are not blocked
                           .AllowCredentials(); // ✅ Only works with explicit origins, not "*"
                });
            });

            return services;
        }
    }
}
