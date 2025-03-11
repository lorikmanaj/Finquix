namespace FinquixAPI.Models
{
    public class FinancialData
    {
        public int Id { get; set; }
        public int UserProfileId { get; set; }
        public decimal Income { get; set; }
        public decimal FixedExpenses { get; set; }
        public decimal VariableExpenses { get; set; }
        public double SavingsRate { get; set; } // Stored as a percentage (e.g., 0.15 for 15%)
        public string RiskTolerance { get; set; } // Low, Medium, High
        //Add Balance

        // Navigation property
        public UserProfile UserProfile { get; set; }
    }
}
