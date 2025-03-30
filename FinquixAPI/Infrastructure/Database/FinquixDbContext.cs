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

        //Seed Questions
        public static void SeedQuestionsIfMissing(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FinquixDbContext>();

            var predefinedQuestions = new List<Question>
            {
                new() { Category = "Personal", Text = "Hi, can you please tell me how much money I have available for investing?" },
                new() { Category = "Personal", Text = "Is it possible for me to grow my money?" },
                new() { Category = "Investments", Text = "I am an aggressive investor, how much money should I invest? (10-25% of total balance based on the goals I have)" },
                new() { Category = "Investments", Text = "How many % of my available money do you recommend for me to invest in cryptocurrency?" },
                new() { Category = "Crypto", Text = "Which cryptocurrency should I invest in?" },
                new() { Category = "Stocks", Text = "Which stock option should I invest in?" }
            };

            foreach (var question in predefinedQuestions)
            {
                if (!context.Questions.Any(q => q.Text == question.Text))
                    context.Questions.Add(question);
            }

            context.SaveChanges();
        }
    }
}
