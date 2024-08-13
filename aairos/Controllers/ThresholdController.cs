using aairos.Data;
using aairos.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThresholdController : ControllerBase
    {
        private readonly ThresholdContext _context;
        private readonly sensor_dataContext _sensorDataContext;

        public ThresholdController(ThresholdContext context, sensor_dataContext sensorDataContext)
        {
            _context = context;
            _sensorDataContext = sensorDataContext;
        }

        // GET: api/Threshold
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Threshold>>> GetAllThresholds()
        {
            var thresholds = await _context.Threshold.ToListAsync();
            return Ok(thresholds);
        }

        // GET: api/Threshold/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Threshold>> GetThresholdById(int id)
        {
            var threshold = await _context.Threshold.FindAsync(id);

            if (threshold == null)
            {
                return NotFound();
            }

            return Ok(threshold);
        }

        // POST: api/Threshold/CreateSingle
        [HttpPost("CreateSingle")]
        public async Task<IActionResult> CreateSingleThreshold(int userProfileId, int deviceId, int Threshold_1, int Threshold_2)
        {
            var threshold = new Threshold
            {
                userProfileId = userProfileId,
                deviceId = deviceId,
                Threshold_1 = Threshold_1,
                Threshold_2 = Threshold_2,
                createdDateTime = DateTime.UtcNow.ToString(), // Ensures it's stored as a string
                updatedDateTime = DateTime.UtcNow
            };

            _context.Threshold.Add(threshold);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetThresholdById), new { id = threshold.Id }, threshold);
        }

        // PUT: api/Threshold/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateThreshold(int id, int userProfileId, int deviceId, int Threshold_1, int Threshold_2)
        {
            var threshold = await _context.Threshold.FindAsync(id);

            if (threshold == null)
            {
                return NotFound();
            }

            // Update the fields
            threshold.userProfileId = userProfileId;
            threshold.deviceId = deviceId;
            threshold.Threshold_1 = Threshold_1;
            threshold.Threshold_2 = Threshold_2;
            threshold.updatedDateTime = DateTime.UtcNow; // Update only the updatedDateTime

            // Save changes to the database
            _context.Entry(threshold).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent(); // Indicates the update was successful without returning any content
        }
    }
}
