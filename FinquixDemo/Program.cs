
using FinquixDemo.Infrastructure;

namespace FinquixDemo
{
    public class Program
    {
        //public static void Main(string[] args)
        //{
        //    var builder = WebApplication.CreateBuilder(args);

        //    // Add services to the container.

        //    builder.Services.AddControllers();
        //    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        //    builder.Services.AddEndpointsApiExplorer();
        //    builder.Services.AddSwaggerGen();

        //    var app = builder.Build();

        //    // Configure the HTTP request pipeline.
        //    if (app.Environment.IsDevelopment())
        //    {
        //        app.UseSwagger();
        //        app.UseSwaggerUI();
        //    }

        //    app.UseHttpsRedirection();

        //    app.UseAuthorization();


        //    app.MapControllers();

        //    app.Run();
        //}

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure the application to load environment-specific settings
            ConfigureAppConfiguration(builder);

            ConfigureServices(builder.Services, builder.Configuration);

            builder.Services.AddHttpClient();

            // Add logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

            var app = builder.Build();
            //app.RunMigrations();

            ConfigureApp(app);

            app.Run();
        }

        private static void ConfigureAppConfiguration(WebApplicationBuilder builder)
        {
            var environment = builder.Environment.EnvironmentName;

            // Load the appsettings.json first
            //builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //                     .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

            // If the environment is "Local", load appsettings.Local.json
            //if (environment == "Local")
            //    builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

            builder.Configuration.AddEnvironmentVariables();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddInfrastructure(configuration);

            services.AddSwagger();

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
            app.UseHttpsRedirection();
            app.UseRouting();

            app.MapControllers();
        }
    }
}
