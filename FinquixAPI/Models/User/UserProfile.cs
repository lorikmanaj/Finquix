using FinquixAPI.Models.Financials;

namespace FinquixAPI.Models.User
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string EmploymentStatus { get; set; }
        public int Dependents { get; set; }

        public string InvestmentBehavior { get; set; } // Conservative, Moderate, Aggressive

        // Navigation properties
        public ICollection<FinancialGoal> FinancialGoals { get; set; }
        public FinancialData FinancialData { get; set; }
    }
}
