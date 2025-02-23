﻿using FinquixDemo.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace FinquixDemo.Infrastructure
{
    public static class WebApplicationExtensions
    {
        public static WebApplication RunMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FinquixDbContext>();

            context.Database.Migrate();
            context.Database.EnsureCreated();

            return app;
        }
    }
}
