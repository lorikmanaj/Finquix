using FinquixAPI.Models.AI;
using FinquixAPI.Models.Assets;
using FinquixAPI.Models.Financials;
using FinquixAPI.Models.User;
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
            modelBuilder.Entity<FinancialData>().Property(f => f.TotalNetWorth)
                .HasComputedColumnSql("[Savings] + [Investments] - [Debt]", stored: true); // ✅ Mark as a stored column

            modelBuilder.Entity<FinancialGoal>().Property(f => f.ProgressPercentage)
                .HasComputedColumnSql("(100.0 * [CurrentProgress] / NULLIF([EstimatedValue], 0))", stored: true);

            //modelBuilder.Entity<StockAsset>().HasData(
            //    new StockAsset { Id = 1, Symbol = "AAPL", Name = "Apple Inc.", CurrentPrice = 150.23m, ChangePercent = 0, LastUpdated = DateTime.UtcNow },
            //    new StockAsset { Id = 2, Symbol = "GOOGL", Name = "Alphabet Inc.", CurrentPrice = 2725.67m, ChangePercent = 0, LastUpdated = DateTime.UtcNow },
            //    new StockAsset { Id = 3, Symbol = "MSFT", Name = "Microsoft Corp.", CurrentPrice = 299.12m, ChangePercent = 0, LastUpdated = DateTime.UtcNow }
            //);
        }
    }
}
