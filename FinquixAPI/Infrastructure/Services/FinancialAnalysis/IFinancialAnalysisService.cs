using FinquixAPI.Models.Financials;
using FinquixAPI.Models.User;

namespace FinquixAPI.Infrastructure.Services.FinancialAnalysis
{
    public interface IFinancialAnalysisService
    {
        Task<FinancialOverview> AnalyzeUserFinances(int userId);
        Task<IEnumerable<UserProfile>> GetAllUserProfilesAsync();
        Task<UserProfile> GetUserProfileByIdAsync(int id);
        Task<UserProfile> CreateUserProfileAsync(UserProfile userProfile);
        Task<UserProfile> UpdateUserProfileAsync(UserProfile userProfile);
    }
}
