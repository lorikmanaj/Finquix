namespace FinquixAPI.Models
{
    public class CryptoAsset
    {
        public int Id { get; set; }
        public string Symbol { get; set; } // BTC, ETH
        public string Name { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal MarketCap { get; set; }
        public decimal ChangePercent { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
