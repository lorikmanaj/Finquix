namespace FinquixAPI.Models.Assets
{
    public class CryptoQuote
    {
        public string Symbol { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal ChangePercentage { get; set; }
        public decimal Change { get; set; }
        public string Exchange { get; set; }
    }
}
