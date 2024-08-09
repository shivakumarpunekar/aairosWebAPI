using aairos.Data;
using aairos.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
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

        // POST: api/Threshold
        [HttpPost]
        public async Task<IActionResult> CreateThresholds(int profileId, int deviceId)
        {
            // Fetch the most recent sensor data
            var sensorData = await _sensorDataContext.sensor_data
                .Where(sd => sd.deviceId == deviceId)
                .OrderByDescending(sd => sd.timestamp)
                .FirstOrDefaultAsync();

            if (sensorData == null)
            {
                return NotFound("No sensor data found for the specified device.");
            }

            // Determine the threshold values based on the sensor data
            var threshold = new Threshold
            {
                profileId = profileId,
                deviceId = deviceId,
                Threshold_1 = (sensorData.sensor1_value <= 1250 || sensorData.sensor1_value >= 4000) ? sensorData.sensor1_value : 0,
                Threshold_2 = (sensorData.sensor2_value <= 1250 || sensorData.sensor2_value >= 4000) ? sensorData.sensor2_value : 0,
                createdDateTime = sensorData.createdDateTime,
                updatedDateTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            // Save the threshold to the database
            _context.Threshold.Add(threshold);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetThresholdById), new { id = threshold.Id }, threshold);
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

            return threshold;
        }
    }
}
