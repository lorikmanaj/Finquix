namespace FinquixDemo.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDbContext<ApplicationDbContext>(options =>
            //{
            //    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
            //        b => b.MigrationsAssembly("ProducingAPI"));
            //});

            services.AddHttpContextAccessor();
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
    }
}
