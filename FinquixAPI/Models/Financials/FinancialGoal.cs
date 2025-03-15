using FinquixAPI.Models.User;

namespace FinquixAPI.Models.Financials
{
    public class FinancialGoal
    {
        public int Id { get; set; }
        public int UserProfileId { get; set; }
        public string GoalType { get; set; }  // DebtRepayment, HousePurchase, Retirement
        public bool IsActive { get; set; }

        public decimal EstimatedValue { get; set; } // Example: 100000 for a house
        public decimal CurrentProgress { get; set; } // How much has been saved?
        public decimal MonthlyContribution { get; set; } // How much user saves per month?

        public DateTime StartDate { get; set; } // When goal was started
        public DateTime TargetDate { get; set; } // When the user wants to reach it

        private decimal _progressPercentage;
        public decimal ProgressPercentage
        {
            get => EstimatedValue > 0 ? (CurrentProgress / EstimatedValue) * 100 : 0;
            private set => _progressPercentage = value; // Backing field for EF Core
        }

        // Navigation property
        public UserProfile UserProfile { get; set; }
    }
}
