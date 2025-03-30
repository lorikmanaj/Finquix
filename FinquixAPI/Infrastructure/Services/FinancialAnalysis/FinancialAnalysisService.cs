using FinquixAPI.Infrastructure.Database;
using FinquixAPI.Models.Financials;
using FinquixAPI.Models.User;
using Microsoft.EntityFrameworkCore;

namespace FinquixAPI.Infrastructure.Services.FinancialAnalysis
{
    public class FinancialAnalysisService(FinquixDbContext context) : IFinancialAnalysisService
    {
        private readonly FinquixDbContext _context = context;

        public async Task<FinancialOverview> AnalyzeUserFinances(int userId)
        {
            var user = await _context.UserProfiles
                .Include(u => u.FinancialGoals)
                .Include(u => u.FinancialData)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return null;

            return new FinancialOverview
            {
                UserName = user.Name,
                Income = user.FinancialData?.Income ?? 0,
                FixedExpenses = user.FinancialData?.FixedExpenses ?? 0,
                VariableExpenses = user.FinancialData?.VariableExpenses ?? 0,
                Savings = user.FinancialData?.Savings ?? 0,
                InvestmentPortfolio = user.FinancialData?.Investments ?? 0,
                DebtToIncomeRatio = user.FinancialData?.Debt / user.FinancialData?.Income ?? 0,
                SavingsRate = user.FinancialData?.Savings / user.FinancialData?.Income ?? 0,
                EmergencyFundScore = user.FinancialData?.EmergencyFund / user.FinancialData?.FixedExpenses ?? 0,
                Goals = user.FinancialGoals?.Select(g => new FinancialGoalSummary
                {
                    GoalType = g.GoalType,
                    EstimatedValue = g.EstimatedValue,
                    CurrentProgress = g.CurrentProgress,
                    MonthlyContribution = g.MonthlyContribution
                }).ToList()
            };
        }

        public async Task<IEnumerable<UserProfile>> GetAllUserProfilesAsync()
        {
            return await _context.UserProfiles.Include(u => u.FinancialGoals)
                                              .Include(u => u.FinancialData)
                                              .ToListAsync();
        }

        public async Task<UserProfile> GetUserProfileByIdAsync(int id)
        {
            return await _context.UserProfiles.Include(u => u.FinancialGoals)
                                              .Include(u => u.FinancialData)
                                              .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<UserProfile> CreateUserProfileAsync(UserProfile userProfile)
        {
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();
            return userProfile;
        }

        public async Task<UserProfile> UpdateUserProfileAsync(UserProfile userProfile)
        {
            // Check if user exists
            var existingUser = await _context.UserProfiles
                .Include(u => u.FinancialGoals)
                .Include(u => u.FinancialData)
                .FirstOrDefaultAsync(u => u.Id == userProfile.Id);

            if (existingUser == null)
                return null;

            // Update basic profile properties
            _context.Entry(existingUser).CurrentValues.SetValues(userProfile);

            // Update financial data if it exists
            if (userProfile.FinancialData != null)
                if (existingUser.FinancialData == null)
                    existingUser.FinancialData = userProfile.FinancialData;
                else
                    _context.Entry(existingUser.FinancialData).CurrentValues.SetValues(userProfile.FinancialData);

            // Update financial goals
            if (userProfile.FinancialGoals != null)
            {
                foreach (var existingGoal in existingUser.FinancialGoals.ToList())
                    if (!userProfile.FinancialGoals.Any(g => g.Id == existingGoal.Id))
                        _context.FinancialGoals.Remove(existingGoal);

                // Update existing goals and add new ones
                foreach (var goal in userProfile.FinancialGoals)
                {
                    var existingGoal = existingUser.FinancialGoals
                        .FirstOrDefault(g => g.Id == goal.Id);

                    if (existingGoal != null)
                        _context.Entry(existingGoal).CurrentValues.SetValues(goal);
                    else
                        existingUser.FinancialGoals.Add(goal);
                }
            }

            await _context.SaveChangesAsync();
            return existingUser;
        }
    }
}
