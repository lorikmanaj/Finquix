namespace FinquixAPI.Models
{
    public class FinancialGoal
    {
        public int Id { get; set; }
        public int UserProfileId { get; set; }
        public string GoalType { get; set; }  // DebtRepayment, HousePurchase, Retirement, EducationSavings
        public bool IsActive { get; set; }

        //EstimatedValue 100000
        //Budget

        // Navigation property
        public UserProfile UserProfile { get; set; }
    }
}
