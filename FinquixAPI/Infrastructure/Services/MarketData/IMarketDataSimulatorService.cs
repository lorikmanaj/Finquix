using FinquixAPI.Models.Assets;
using FinquixAPI.Models.Financials;

namespace FinquixAPI.Infrastructure.Services.MarketData
{
    public interface IMarketDataSimulatorService
    {
        Task<List<CryptoAsset>> GetCryptoAssetsAsync();
        Task<List<FinancialSignal>> GetFinancialSignalsAsync();
    }
}
