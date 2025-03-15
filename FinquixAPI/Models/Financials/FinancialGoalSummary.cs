namespace FinquixAPI.Models.Financials
{
    public class FinancialGoalSummary
    {
        public string GoalType { get; set; }
        public decimal EstimatedValue { get; set; }
        public decimal CurrentProgress { get; set; }
        public decimal MonthlyContribution { get; set; }
    }
}
