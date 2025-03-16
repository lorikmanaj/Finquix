using FinquixAPI.Infrastructure.Services.FinancialAnalysis;
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
    }
}