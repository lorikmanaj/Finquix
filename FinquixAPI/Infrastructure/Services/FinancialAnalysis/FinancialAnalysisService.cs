using FinquixAPI.Infrastructure.Database;
using FinquixAPI.Models.Financials;
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
    }
}
