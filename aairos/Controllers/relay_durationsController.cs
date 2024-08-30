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

        // GET: api/relay_durations/Device/{user_id}
        [HttpGet("Device/{user_id}")]
        public async Task<ActionResult<IEnumerable<relay_durations>>> GetDeviceId(int user_id)
        {
            var relay_durations = await _context.relay_durations
                                                .Where(r => r.user_id == user_id)
                                                .OrderByDescending(r => r.timestamp)
                                                .ToListAsync();

            if (relay_durations == null || !relay_durations.Any())
            {
                return NotFound();
            }

            return Ok(relay_durations);
        }
    }
}
