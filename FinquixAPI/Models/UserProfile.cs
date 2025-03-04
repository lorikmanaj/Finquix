namespace FinquixAPI.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string EmploymentStatus { get; set; }
        public int Dependents { get; set; }

        // Navigation properties
        public ICollection<FinancialGoal> FinancialGoals { get; set; }
        public FinancialData FinancialData { get; set; }
    }
}
