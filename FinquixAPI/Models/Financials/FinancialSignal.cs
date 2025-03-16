namespace FinquixAPI.Models.Financials
{
    public class FinancialSignal
    {
        public int Id { get; set; }
        public string Ticker { get; set; } // Example: AAPL, GOOGL
        public string CompanyName { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal PreviousPrice { get; set; } // Last recorded price
        public decimal ChangePercent { get; set; }//=> ((CurrentPrice - PreviousPrice) / PreviousPrice) * 100; // Computed
        public DateTime SignalDate { get; set; }
        public string Recommendation { get; set; } // Buy, Hold, Sell
        public decimal PredictedChange { get; set; } // AI-predicted percentage movement
    }
}
