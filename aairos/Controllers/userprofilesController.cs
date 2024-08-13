using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aairos.Data;
using aairos.Model;
using aairos.Services;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userprofilesController : ControllerBase
    {
        //This is a Log
/*        private readonly FileLoggerService _logger;
*/        private readonly userprofileContext _context;

        public userprofilesController(userprofileContext context, FileLoggerService logger)
        {
/*            _logger = logger;
*/            _context = context;
        }

        // GET: api/userprofiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<userprofile>>> Getuserprofile()
        {
            /*return await  _context.UserProfile.ToListAsync();*/

            var userprofiles = await _context.UserProfile.ToListAsync();

/*            await _logger.LogAsync($"GET: api/userprofiles returned {userprofiles.Count} records.");
*/
            return userprofiles;
        }

        // GET: api/userprofileByName
        [HttpGet("GetuserprofileByName")]
        public async Task<ActionResult<IEnumerable<object>>> GetuserprofileByName()
        {
            var userprofiles = await _context.UserProfile
                .Select(up => new
                {
                    up.userProfileId,
                    up.FirstName,
                    up.MiddleName,
                    up.LastName
                })
                .ToListAsync();

            return Ok(userprofiles);
        }



        //This is a guId GET Methode
        [HttpGet("byGuId/{guId}")]
        public async Task<ActionResult<userprofile>> GetUserProfileByGuId(string GuId)
        {
            var userprofile = await _context.UserProfile.FirstOrDefaultAsync(u => u.GuId == GuId);

            if (userprofile == null)
            {
/*                await _logger.LogAsync($"GET: api/userprofiles/byGuId/{GuId} returned NotFound.");
*/                return NotFound();
            }

/*            await _logger.LogAsync($"GET: api/userprofiles/byGuId/{GuId} returned a user profile.");
*/            return userprofile;
        }

        // GET: api/userprofiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<userprofile>> Getuserprofile(int id)
        {
            var userprofile = await _context.UserProfile.FindAsync(id);

            if (userprofile == null)
            {
/*                await _logger.LogAsync($"GET: api/userprofiles/{id} returned NotFound.");
*/                return NotFound();
            }

            // Update the UpdatedDate field
            userprofile.UpdatedDate = DateTime.UtcNow;

            _context.Entry(userprofile).State = EntityState.Modified;
            await _context.SaveChangesAsync();

/*            await _logger.LogAsync($"GET: api/userprofiles/{id} returned a user profile.");
*/            return userprofile;
        }


        // GET: api/userprofiles/registrationsSummary
        [HttpGet("registrationsSummary")]
        public async Task<ActionResult<IEnumerable<RegistrationSummary>>> GetRegistrationsSummary()
        {
            var summary = await _context.UserProfile
               .GroupBy(u => u.CreatedDate.Date)
               .Select(g => new RegistrationSummary
               {
                   CreatedDate = g.Key,
                   Count = g.Count()
               })
               .ToListAsync();

/*            await _logger.LogAsync($"GET: api/userprofiles/registrationsSummary returned {summary.Count} records.");
*/            return Ok(summary);
        }

        public class RegistrationSummary
        {
            public DateTime CreatedDate { get; set; }
            public int Count { get; set; }
        }



        // PUT: api/userprofiles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Putuserprofile(int id, userprofile userprofile)
        {
            if (id != userprofile.userProfileId)
            {
/*                await _logger.LogAsync($"PUT: api/userprofiles/{id} returned BadRequest due to mismatched IDs.");
*/                return BadRequest();
            }

            // Retrieve the existing user profile
            var existingProfile = await _context.UserProfile.FindAsync(id);
            if (existingProfile == null)
            {
/*                await _logger.LogAsync($"PUT: api/userprofiles/{id} returned NotFound.");
*/                return NotFound();
            }

            try
            {
                // Detach the existing profile from the context
                _context.Entry(existingProfile).State = EntityState.Detached;

                // Preserve the original CreateDate
                userprofile.CreatedDate = existingProfile.CreatedDate;
                userprofile.UpdatedDate = DateTime.UtcNow;

                // Mark the new profile as modified
                _context.Entry(userprofile).State = EntityState.Modified;

                await _context.SaveChangesAsync();
/*                await _logger.LogAsync($"PUT: api/userprofiles/{id} updated successfully.");
*/            }
            catch (DbUpdateConcurrencyException)
            {
                if (!userprofileExists(id))
                {
/*                    await _logger.LogAsync($"PUT: api/userprofiles/{id} returned NotFound during concurrency check.");
*/                    return NotFound();
                }
                else
                {
/*                    await _logger.LogAsync($"PUT: api/userprofiles/{id} encountered a concurrency exception.");
*/                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/userprofiles
        [HttpPost]
        public async Task<ActionResult<userprofile>> Postuserprofile(userprofile userprofile)
        {
            // Check if UserName or MobileNumber already exists
            var existingUserName = await _context.UserProfile.AnyAsync(u => u.UserName == userprofile.UserName);
            var existingMobileNumber = await _context.UserProfile.AnyAsync(u => u.MobileNumber == userprofile.MobileNumber);

            if (existingUserName)
            {
/*                await _logger.LogAsync("POST: api/userprofiles returned Conflict due to existing UserName.");
*/                return Conflict(new { message = "UserName already exists." });
            }

            if (existingMobileNumber)
            {
/*                await _logger.LogAsync("POST: api/userprofiles returned Conflict due to existing MobileNumber.");
*/                return Conflict(new { message = "MobileNumber already exists." });
            }

            userprofile.CreatedDate = DateTime.UtcNow;

            _context.UserProfile.Add(userprofile);
            await _context.SaveChangesAsync();

/*            await _logger.LogAsync($"POST: api/userprofiles created a new user profile with ID {userprofile.userProfileId}.");
*/            return CreatedAtAction(nameof(Getuserprofile), new { id = userprofile.userProfileId }, userprofile);
        }

        // DELETE: api/userprofiles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleteuserprofile(int id)
        {
            var userprofile = await _context.UserProfile.FindAsync(id);
            if (userprofile == null)
            {
/*                await _logger.LogAsync($"DELETE: api/userprofiles/{id} returned NotFound.");
*/                return NotFound();
            }

            _context.UserProfile.Remove(userprofile);
            await _context.SaveChangesAsync();

/*            await _logger.LogAsync($"DELETE: api/userprofiles/{id} deleted successfully.");
*/            return NoContent();
        }

        private bool userprofileExists(int id)
        {
            return _context.UserProfile.Any(e => e.userProfileId == id);
        }
    }
}
