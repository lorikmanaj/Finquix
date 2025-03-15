using FinquixAPI.Models.Financials;

namespace FinquixAPI.Infrastructure.Services.FinancialAnalysis
{
    public interface IFinancialAnalysisService
    {
        Task<FinancialOverview> AnalyzeUserFinances(int userId);
    }
}
