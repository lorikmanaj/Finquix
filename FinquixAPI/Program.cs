
using FinquixAPI.Infrastructure;

namespace FinquixAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            ConfigureAppConfiguration(builder);

            ConfigureServices(builder.Services, builder.Configuration);

            builder.Services.AddHttpClient();

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            var app = builder.Build();
            app.RunMigrations();

            ConfigureApp(app);

            app.Run();
        }

        private static void ConfigureAppConfiguration(WebApplicationBuilder builder)
        {
            var environment = builder.Environment.EnvironmentName;

            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                 .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

            if (environment == "Local")
                builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

            builder.Configuration.AddEnvironmentVariables();
        }

        private static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddInfrastructure(configuration);

            var fmpApiKey = configuration["FMP_API_KEY"];
            if (string.IsNullOrEmpty(fmpApiKey))
            {
                throw new ArgumentNullException(nameof(fmpApiKey), "FMP API Key is missing in configuration.");
            }

            services.AddSwagger();

            services.AddCustomCors(configuration);

            services.AddControllers()
                    .AddJsonOptions(options =>
                        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
        }
        private static void ConfigureApp(WebApplication app)
        {
            if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Local"))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //CORS should come before `UseRouting()`
            app.UseCors("AllowSpecificOrigins");

            app.UseHttpsRedirection();
            app.UseRouting();

            app.MapControllers();
        }
    }
}
