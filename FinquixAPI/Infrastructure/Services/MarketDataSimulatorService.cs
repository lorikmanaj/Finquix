using FinquixAPI.Infrastructure.Database;
using FinquixAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FinquixAPI.Infrastructure.Services
{
    public class MarketDataSimulatorService(IServiceProvider serviceProvider) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly Random _random = new();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await GenerateAndUpdateMarketData();
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Simulate every 30s
            }
        }

        private async Task GenerateAndUpdateMarketData()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FinquixDbContext>();

            // ✅ Generate fake assets if they don't exist
            if (!await context.CryptoAssets.AnyAsync())
            {
                var fakeAssets = new List<CryptoAsset>
            {
                new CryptoAsset { Symbol = "BTC", Name = "Bitcoin", CurrentPrice = 30000 },
                new CryptoAsset { Symbol = "ETH", Name = "Ethereum", CurrentPrice = 2000 },
                new CryptoAsset { Symbol = "DOGE", Name = "Dogecoin", CurrentPrice = 0.25M }
            };

                context.CryptoAssets.AddRange(fakeAssets);
            }

            if (!await context.FinancialSignals.AnyAsync())
            {
                var fakeStocks = new List<FinancialSignal>
            {
                new FinancialSignal { Ticker = "AAPL", CompanyName = "Apple Inc.", CurrentPrice = 150 },
                new FinancialSignal { Ticker = "GOOGL", CompanyName = "Alphabet Inc.", CurrentPrice = 2800 },
                new FinancialSignal { Ticker = "TSLA", CompanyName = "Tesla Inc.", CurrentPrice = 700 }
            };

                context.FinancialSignals.AddRange(fakeStocks);
            }

            await context.SaveChangesAsync();

            // ✅ Simulate market data updates
            var cryptoAssets = await context.CryptoAssets.ToListAsync();
            var financialSignals = await context.FinancialSignals.ToListAsync();

            foreach (var asset in cryptoAssets)
            {
                asset.CurrentPrice += (decimal)(_random.NextDouble() * 100 - 50);
                asset.ChangePercent = (decimal)(_random.NextDouble() * 10 - 5);
                asset.LastUpdated = DateTime.UtcNow;
            }

            foreach (var signal in financialSignals)
            {
                signal.CurrentPrice += (decimal)(_random.NextDouble() * 20 - 10);
                signal.ChangePercent = (decimal)(_random.NextDouble() * 5 - 2.5);
                signal.SignalDate = DateTime.UtcNow;
                signal.Recommendation = _random.Next(0, 3) switch
                {
                    0 => "Buy",
                    1 => "Hold",
                    _ => "Sell"
                };
            }

            await context.SaveChangesAsync();
        }
    }
}
