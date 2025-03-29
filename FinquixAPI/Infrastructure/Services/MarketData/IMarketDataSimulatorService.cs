using FinquixAPI.Models.Assets;
using FinquixAPI.Models.Financials;

namespace FinquixAPI.Infrastructure.Services.MarketData
{
    public interface IMarketDataSimulatorService
    {
        Task GenerateAndUpdateMarketData();
        Task<List<CryptoAsset>> GetCryptoAssetsAsync();
        Task<List<FinancialSignal>> GetFinancialSignalsAsync();
        Task<List<FinancialSignal>> GetLatestStockDataAsync();
        Task<List<CryptoAsset>> GetLatestCryptoDataAsync();
    }
}
