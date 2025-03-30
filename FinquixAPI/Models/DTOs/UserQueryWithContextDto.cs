using FinquixAPI.Models.Assets;
using FinquixAPI.Models.Financials;
using FinquixAPI.Models.User;

namespace FinquixAPI.Models.DTOs
{
    public class UserQueryWithContextDto : UserQuery
    {
        public List<FinancialSignal> CurrentStockData { get; set; }
        public List<CryptoAsset> CurrentCryptoData { get; set; }
    }
}
