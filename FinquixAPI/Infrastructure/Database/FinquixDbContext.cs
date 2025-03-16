using FinquixAPI.Models;
using FinquixAPI.Models.AI;
using Microsoft.EntityFrameworkCore;

namespace FinquixAPI.Infrastructure.Database
{
    public class FinquixDbContext(DbContextOptions<FinquixDbContext> options) : DbContext(options)
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<FinancialGoal> FinancialGoals { get; set; }
        public DbSet<FinancialData> FinancialData { get; set; }
        public DbSet<FinancialSignal> FinancialSignals { get; set; }
        public DbSet<CryptoAsset> CryptoAssets { get; set; }
        public DbSet<StockAsset> StockAssets { get; set; }

        public DbSet<Question> Questions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<StockAsset>().HasData(
            //    new StockAsset { Id = 1, Symbol = "AAPL", Name = "Apple Inc.", CurrentPrice = 150.23m, ChangePercent = 0, LastUpdated = DateTime.UtcNow },
            //    new StockAsset { Id = 2, Symbol = "GOOGL", Name = "Alphabet Inc.", CurrentPrice = 2725.67m, ChangePercent = 0, LastUpdated = DateTime.UtcNow },
            //    new StockAsset { Id = 3, Symbol = "MSFT", Name = "Microsoft Corp.", CurrentPrice = 299.12m, ChangePercent = 0, LastUpdated = DateTime.UtcNow }
            //);
        }
    }
}
