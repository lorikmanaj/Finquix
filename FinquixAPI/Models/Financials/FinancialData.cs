using FinquixAPI.Models.User;

namespace FinquixAPI.Models.Financials
{
    public class FinancialData
    {
        public int Id { get; set; }
        public int UserProfileId { get; set; }

        public decimal Income { get; set; }
        public decimal FixedExpenses { get; set; }
        public decimal VariableExpenses { get; set; }
        public decimal Savings { get; set; } // Total savings balance
        public decimal Investments { get; set; } // Money put into assets
        public decimal Debt { get; set; } // Outstanding debt
        public decimal EmergencyFund { get; set; } // How much saved for emergencies
        public decimal MonthlyBudget => Income - (FixedExpenses + VariableExpenses); // Computed Property

        private decimal _totalNetWorth;
        public decimal TotalNetWorth
        {
            get => Savings + Investments - Debt; // Computed in memory
            private set => _totalNetWorth = value; // Required by EF
        }

        public string RiskTolerance { get; set; } // Low, Medium, High

        // Navigation property
        public UserProfile UserProfile { get; set; }
    }
}
