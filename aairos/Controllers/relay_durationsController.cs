using aairos.Data;
using aairos.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class relay_durationsController : ControllerBase
    {
        private readonly relay_durationsContext _context;

        public relay_durationsController(relay_durationsContext context)
        {
            _context = context;
        }

        // GET: api/relay_durations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<relay_durations>>> GetAllrelay_durations()
        {
            var relay_durations = await _context.relay_durations.ToListAsync();
            return Ok(relay_durations);
        }

        [HttpGet("Device/{user_id}")]
        public async Task<ActionResult<IEnumerable<relay_durations>>> GetDeviceId(int user_id)
        {
            var query = @"
                        SELECT *, 
                               CONVERT_TZ(timestamp, '+00:00', '+05:30') AS last_updated
                        FROM relay_durations
                        WHERE user_id = @p0
                        ORDER BY last_updated DESC
                        LIMIT 0, 500;";

            var relayDurations = await _context.relay_durations
                .FromSqlRaw(query, user_id)
                .ToListAsync();

            if (relayDurations == null || !relayDurations.Any())
            {
                return NotFound();
            }

            // Return the results mapped to the DTO
            return Ok(relayDurations);
        }

    }
}
