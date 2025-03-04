namespace FinquixAPI.Models
{
    public class FinancialSignal
    {
        public int Id { get; set; }
        public string Ticker { get; set; } // Example: AAPL, GOOGL
        public string CompanyName { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal ChangePercent { get; set; }
        public DateTime SignalDate { get; set; }
        public string Recommendation { get; set; } // Buy, Hold, Sell
    }
}
