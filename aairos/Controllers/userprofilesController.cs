﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aairos.Data;
using aairos.Model;
using Microsoft.AspNetCore.Authorization;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class userprofilesController : ControllerBase
    {
        private readonly userprofileContext _context;

        public userprofilesController(userprofileContext context)
        {
            _context = context;
        }

        // GET: api/userprofiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<userprofile>>> Getuserprofile()
        {
            return await _context.UserProfile.ToListAsync();
        }

        //This is a guId GET Methode
        [HttpGet("byGuId/{guId}")]
        public async Task<ActionResult<userprofile>> GetUserProfileByGuId(string GuId)
        {
            var userprofile = await _context.UserProfile.FirstOrDefaultAsync(u => u.GuId == GuId);

            if (userprofile == null)
            {
                return NotFound();
            }

            return userprofile;
        }

        // GET: api/userprofiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<userprofile>> Getuserprofile(int id)
        {
            var userprofile = await _context.UserProfile.FindAsync(id);

            if (userprofile == null)
            {
                return NotFound();
            }

            // Update the UpdatedDate field
            userprofile.UpdatedDate = DateTime.UtcNow;

            _context.Entry(userprofile).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return userprofile;
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

            return Ok(summary);
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
            if (id != userprofile.UserProfileId)
            {
                return BadRequest();
            }

            // Retrieve the existing user profile
            var existingProfile = await _context.UserProfile.FindAsync(id);
            if (existingProfile == null)
            {
                return NotFound();
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
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!userprofileExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
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
                return Conflict(new { message = "UserName already exists." });
            }

            if (existingMobileNumber)
            {
                return Conflict(new { message = "MobileNumber already exists." });
            }

            userprofile.CreatedDate = DateTime.UtcNow;

            _context.UserProfile.Add(userprofile);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Getuserprofile), new { id = userprofile.UserProfileId }, userprofile);
        }

        // DELETE: api/userprofiles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleteuserprofile(int id)
        {
            var userprofile = await _context.UserProfile.FindAsync(id);
            if (userprofile == null)
            {
                return NotFound();
            }

            _context.UserProfile.Remove(userprofile);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool userprofileExists(int id)
        {
            return _context.UserProfile.Any(e => e.UserProfileId == id);
        }
    }
}
