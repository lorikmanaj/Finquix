using FinquixAPI.Infrastructure.Services.FinancialAnalysis;
using FinquixAPI.Models.DTOs;
using FinquixAPI.Models.Financials;
using FinquixAPI.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace FinquixAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController(IFinancialAnalysisService financialAnalysisService) : ControllerBase
    {
        private readonly IFinancialAnalysisService _financialAnalysisService = financialAnalysisService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProfile>>> GetUserProfiles()
        {
            return Ok(await _financialAnalysisService.GetAllUserProfilesAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfile>> GetUserProfile(int id)
        {
            var userProfile = await _financialAnalysisService.GetUserProfileByIdAsync(id);
            if (userProfile == null) return NotFound();
            return Ok(userProfile);
        }

        [HttpPost]
        public async Task<ActionResult<UserProfile>> PostUserProfile(UserProfile userProfile)
        {
            var createdProfile = await _financialAnalysisService.CreateUserProfileAsync(userProfile);
            return CreatedAtAction(nameof(GetUserProfile), new { id = createdProfile.Id }, createdProfile);
        }

        [HttpPost("submit-onboarding")]
        public async Task<IActionResult> SubmitOnboarding([FromBody] UserProfile userProfile)
        {
            var createdProfile = await _financialAnalysisService.CreateUserProfileAsync(userProfile);
            return Ok(createdProfile);
        }

        [HttpPut("{userId}/goals")]
        public async Task<IActionResult> UpdateFinancialGoals(int userId, [FromBody] FinancialGoalsUpdateDto updateDto)
        {
            var userProfile = await _financialAnalysisService.GetUserProfileByIdAsync(userId);
            if (userProfile == null) return NotFound();

            userProfile.FinancialGoals = updateDto.FinancialGoals;
            var updatedProfile = await _financialAnalysisService.UpdateUserProfileAsync(userProfile);

            return Ok(updatedProfile);
        }

        [HttpPost("{userId}/goals")]
        public async Task<IActionResult> AddFinancialGoal(int userId, [FromBody] FinancialGoal newGoal)
        {
            var userProfile = await _financialAnalysisService.GetUserProfileByIdAsync(userId);
            if (userProfile == null) return NotFound();

            userProfile.FinancialGoals ??= [];
            userProfile.FinancialGoals.Add(newGoal);

            var updatedProfile = await _financialAnalysisService.UpdateUserProfileAsync(userProfile);
            return Ok(updatedProfile);
        }
    }
}