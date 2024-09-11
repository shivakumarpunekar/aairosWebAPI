using aairos.Data;
using aairos.Dto;
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

        [HttpGet("StateDurations/{user_id}")]
        public async Task<ActionResult<IEnumerable<StateDurationDTO>>> GetStateDurations(int user_id)
        {
            var stateDurationsQuery = @"
        WITH state_durations AS (
            SELECT
                a.id,
                a.user_id,
                a.state,
                DATE(a.timestamp) AS day,
                TIMESTAMPDIFF(MINUTE, a.timestamp, COALESCE(b.timestamp, NOW())) AS duration_minutes
            FROM relay_durations a
            LEFT JOIN relay_durations b
            ON a.user_id = b.user_id
            AND a.timestamp < b.timestamp
            AND b.timestamp = (
                SELECT MIN(timestamp)
                FROM relay_durations
                WHERE timestamp > a.timestamp
                AND user_id = a.user_id
            )
            WHERE a.state IS NOT NULL
        )
        SELECT id, user_id, day,
            SUM(CASE WHEN state = 'on' THEN duration_minutes ELSE 0 END) AS TotalDurationOnMinutes,
            SUM(CASE WHEN state = 'off' THEN duration_minutes ELSE 0 END) AS TotalDurationOffMinutes
        FROM state_durations
        WHERE user_id = @p0
        GROUP BY  day
        ORDER BY day";

            // Execute the query and map the result to StateDurationDTO
            var rawResults = await _context
                .StateDurationDTO
                .FromSqlRaw(stateDurationsQuery, user_id)
                .ToListAsync();

            if (!rawResults.Any())
            {
                return NotFound();
            }

            return Ok(rawResults);
        }



    }
}
