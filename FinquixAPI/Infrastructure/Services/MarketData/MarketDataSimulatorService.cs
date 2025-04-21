using FinquixAPI.Infrastructure.Database;
using FinquixAPI.Models.Assets;
using FinquixAPI.Models.Financials;
using Microsoft.EntityFrameworkCore;

namespace FinquixAPI.Infrastructure.Services.MarketData
{
    using System.Collections.Concurrent;
    using System.Threading;

    public class MarketDataSimulatorService(IServiceProvider serviceProvider, FinquixDbContext context)
        : BackgroundService, IMarketDataSimulatorService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly Random _random = new();
        private readonly FinquixDbContext _context = context;
        private readonly SemaphoreSlim _dataLock = new(1, 1);

        // Replace static lists with thread-safe collections
        private readonly ConcurrentDictionary<string, FinancialSignal> _stockDataCache = new();
        private readonly ConcurrentDictionary<string, CryptoAsset> _cryptoDataCache = new();

        public async Task<List<FinancialSignal>> GetLatestStockDataAsync()
        {
            await _dataLock.WaitAsync();
            try
            {
                return [.. _stockDataCache.Values];
            }
            finally
            {
                _dataLock.Release();
            }
        }

        public async Task<List<CryptoAsset>> GetLatestCryptoDataAsync()
        {
            await _dataLock.WaitAsync();
            try
            {
                return [.. _cryptoDataCache.Values];
            }
            finally
            {
                _dataLock.Release();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await GenerateAndUpdateMarketData();
            while (!stoppingToken.IsCancellationRequested)
            {
                await GenerateAndUpdateMarketData();
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }

        public async Task GenerateAndUpdateMarketData()
        {
            await _dataLock.WaitAsync();
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<FinquixDbContext>();

                if (!await context.CryptoAssets.AnyAsync())
                {
                    var fakeAssets = new List<CryptoAsset>
                    {
                        new() { Symbol = "BTC", Name = "Bitcoin", CurrentPrice = 30000 },
                        new() { Symbol = "ETH", Name = "Ethereum", CurrentPrice = 2000 },
                        new() { Symbol = "DOGE", Name = "Dogecoin", CurrentPrice = 0.25M }
                    };
                    context.CryptoAssets.AddRange(fakeAssets);
                }

                if (!await context.FinancialSignals.AnyAsync())
                {
                    var fakeStocks = new List<FinancialSignal>
                    {
                        new() { Ticker = "AAPL", CompanyName = "Apple Inc.", CurrentPrice = 150 },
                        new() { Ticker = "GOOGL", CompanyName = "Alphabet Inc.", CurrentPrice = 2800 },
                        new() { Ticker = "TSLA", CompanyName = "Tesla Inc.", CurrentPrice = 700 }
                    };
                    context.FinancialSignals.AddRange(fakeStocks);
                }

                await context.SaveChangesAsync();

                var cryptoAssets = await context.CryptoAssets.ToListAsync();
                var financialSignals = await context.FinancialSignals.ToListAsync();

                foreach (var asset in cryptoAssets)
                {
                    asset.PreviousPrice = asset.CurrentPrice;
                    asset.CurrentPrice += (decimal)(_random.NextDouble() * 100 - 50);
                    asset.ChangePercent = ((asset.CurrentPrice - asset.PreviousPrice) / asset.PreviousPrice) * 100;
                    asset.LastUpdated = DateTime.UtcNow;

                    decimal totalSupply = _random.Next(1000000, 1000000000);
                    asset.MarketCap = asset.CurrentPrice * totalSupply;

                    _cryptoDataCache.AddOrUpdate(asset.Symbol, asset, (k, v) => asset);
                }

                foreach (var signal in financialSignals)
                {
                    signal.PreviousPrice = signal.CurrentPrice;
                    signal.CurrentPrice += (decimal)(_random.NextDouble() * 20 - 10);
                    signal.ChangePercent = ((signal.CurrentPrice - signal.PreviousPrice) / signal.PreviousPrice) * 100;
                    signal.SignalDate = DateTime.UtcNow;

                    signal.Recommendation = _random.Next(0, 3) switch
                    {
                        0 => "Buy",
                        1 => "Hold",
                        _ => "Sell"
                    };

                    signal.PredictedChange = signal.ChangePercent * (decimal)(_random.NextDouble() * 1.5);

                    // Update the cache
                    _stockDataCache.AddOrUpdate(signal.Ticker, signal, (k, v) => signal);
                }

                await context.SaveChangesAsync();
            }
            finally
            {
                _dataLock.Release();
            }
        }

        public async Task<List<CryptoAsset>> GetCryptoAssetsAsync()
            => await _context.CryptoAssets.ToListAsync();

        public async Task<List<FinancialSignal>> GetFinancialSignalsAsync()
            => await _context.FinancialSignals.ToListAsync();
    }
}
