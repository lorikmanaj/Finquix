namespace FinquixAPI.Models.Assets
{
    public class FmpStock
    {
        public string symbol { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
        public decimal change { get; set; }
        public decimal changePercentage { get; set; }
    }
}
