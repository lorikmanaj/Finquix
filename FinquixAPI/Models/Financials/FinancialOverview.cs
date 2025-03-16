namespace FinquixAPI.Models.Financials
{
    public class FinancialOverview
    {
        public string UserName { get; set; }
        public decimal Income { get; set; }
        public decimal FixedExpenses { get; set; }
        public decimal VariableExpenses { get; set; }
        public decimal Savings { get; set; }
        public decimal InvestmentPortfolio { get; set; }

        // Computed Financial Health Metrics
        public decimal DebtToIncomeRatio { get; set; } // AI uses this to assess risk
        public decimal SavingsRate { get; set; } // Percentage of income saved
        public decimal EmergencyFundScore { get; set; } // How well is emergency fund maintained?

        public List<FinancialGoalSummary> Goals { get; set; }
    }
}
