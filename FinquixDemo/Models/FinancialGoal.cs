﻿namespace FinquixDemo.Models
{
    public class FinancialGoal
    {
        public int Id { get; set; }
        public int UserProfileId { get; set; }
        public string GoalType { get; set; }  // DebtRepayment, HousePurchase, Retirement, EducationSavings
        public bool IsActive { get; set; }

        // Navigation property
        public UserProfile UserProfile { get; set; }
    }
}
