using FinquixAPI.Infrastructure.Database;
using FinquixAPI.Infrastructure.Services.FinancialAnalysis;
using FinquixAPI.Infrastructure.Services.MarketData;
using Microsoft.EntityFrameworkCore;

namespace FinquixAPI.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FinquixDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("FinquixAPI"));
            });

            services.AddHttpClient();

            //Register OpenAI HTTP client
            //var apiKey = configuration["OpenAI:ApiKey"];
            //if (string.IsNullOrEmpty(apiKey))
            //    throw new InvalidOperationException("OpenAI API Key is missing in configuration.");

            //services.AddHttpClient("OpenAI", client =>
            //{
            //    client.BaseAddress = new Uri("https://api.openai.com/v1/");
            //    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            //    client.DefaultRequestHeaders.Add("Accept", "application/json");
            //});

            services.AddHttpContextAccessor();
            //services.AddHostedService<MarketDataSimulatorService>(); //Background Update
            services.AddScoped<IMarketDataSimulatorService, MarketDataSimulatorService>(); //DI
            services.AddScoped<IFinancialAnalysisService, FinancialAnalysisService>();

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
                           .SetIsOriginAllowed(origin => true)
                           .AllowCredentials();
                });
            });

            return services;
        }
    }
}
