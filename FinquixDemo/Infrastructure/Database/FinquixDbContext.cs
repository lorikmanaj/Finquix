using FinquixDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace FinquixDemo.Infrastructure.Database
{
    public class FinquixDbContext(DbContextOptions<FinquixDbContext> options) : DbContext(options)
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<FinancialGoal> FinancialGoals { get; set; }
        public DbSet<FinancialData> FinancialData { get; set; }
        public DbSet<FinancialSignal> FinancialSignals { get; set; }
        public DbSet<CryptoAsset> CryptoAssets { get; set; }
    }
}
