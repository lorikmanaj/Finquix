using FinquixDemo.Infrastructure.Database;
using FinquixDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinquixDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController(FinquixDbContext context) : ControllerBase
    {
        private readonly FinquixDbContext _context = context;

        // GET: api/userprofiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProfile>>> GetUserProfiles()
        {
            return await _context.UserProfiles.Include(u => u.FinancialGoals)
                                              .Include(u => u.FinancialData)
                                              .ToListAsync();
        }

        // GET: api/userprofiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfile>> GetUserProfile(int id)
        {
            var userProfile = await _context.UserProfiles.Include(u => u.FinancialGoals)
                                                         .Include(u => u.FinancialData)
                                                         .FirstOrDefaultAsync(u => u.Id == id);

            if (userProfile == null) return NotFound();
            return userProfile;
        }

        // POST: api/userprofiles
        [HttpPost]
        public async Task<ActionResult<UserProfile>> PostUserProfile(UserProfile userProfile)
        {
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserProfile), new { id = userProfile.Id }, userProfile);
        }
    }
}