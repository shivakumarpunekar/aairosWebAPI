using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aairos.Data;
using aairos.Model;
using aairos.Dto;
using aairos.Services;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class sensor_dataController : ControllerBase
    {
        // This is for logging.
        private readonly FileLoggerService _logger;
        private readonly sensor_dataContext _context;

        public sensor_dataController(sensor_dataContext context, FileLoggerService logger)
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/sensor_data
        [HttpGet]
        public async Task<ActionResult<IEnumerable<sensor_data>>> GetSensorData()
        {
            await _logger.LogAsync("GET: api/sensor_data called.");

            var data = await _context.sensor_data
                .OrderByDescending(s => s.timestamp)
                .Take(100)
                .Select(s => new SensorDataDto
                {
                    id = s.id,
                    sensor1_value = s.sensor1_value,
                    sensor2_value = s.sensor2_value,
                    deviceId = s.deviceId,
                    solenoidValveStatus = s.solenoidValveStatus ? "On" : "Off",
                    timestamp = s.timestamp,
                })
                .ToListAsync();

            await _logger.LogAsync($"GET: api/sensor_data returned {data.Count} records.");

            return Ok(data);
        }

        // GET api/sensor_data/5
        [HttpGet("{id}")]
        public async Task<ActionResult<sensor_data>> GetSensorData(int id)
        {
            await _logger.LogAsync($"GET: api/sensor_data/{id} called.");

            var sensorData = await _context.sensor_data
                .Select(s => new SensorDataDto
                {
                    id = s.id,
                    sensor1_value = s.sensor1_value,
                    sensor2_value = s.sensor2_value,
                    deviceId = s.deviceId,
                    solenoidValveStatus = s.solenoidValveStatus ? "On" : "Off",
                    timestamp = s.timestamp,
                })
                .FirstOrDefaultAsync(s => s.id == id);

            if (sensorData == null)
            {
                await _logger.LogAsync($"GET: api/sensor_data/{id} returned NotFound.");
                return NotFound();
            }

            await _logger.LogAsync($"GET: api/sensor_data/{id} returned a record.");
            return Ok(sensorData);
        }

        // POST api/sensor_data
        [HttpPost]
        public async Task<ActionResult<sensor_data>> PostSensorData([FromBody] sensor_data value)
        {
            await _logger.LogAsync("POST: api/sensor_data called.");

            _context.sensor_data.Add(value);
            await _context.SaveChangesAsync();

            var sensorDataDto = new SensorDataDto
            {
                id = value.id,
                sensor1_value = value.sensor1_value,
                sensor2_value = value.sensor2_value,
                deviceId = value.deviceId,
                solenoidValveStatus = value.solenoidValveStatus ? "On" : "Off",
                timestamp = value.timestamp,
            };

            await _logger.LogAsync($"POST: api/sensor_data created a new record with ID {value.id}.");
            return CreatedAtAction(nameof(GetSensorData), new { id = value.id }, sensorDataDto);
        }

        // PUT api/sensor_data/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSensorData(int id, [FromBody] sensor_data value)
        {
            await _logger.LogAsync($"PUT: api/sensor_data/{id} called.");

            if (id != value.id)
            {
                await _logger.LogAsync($"PUT: api/sensor_data/{id} returned BadRequest due to ID mismatch.");
                return BadRequest();
            }

            _context.Entry(value).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await _logger.LogAsync($"PUT: api/sensor_data/{id} updated successfully.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SensorDataExists(id))
                {
                    await _logger.LogAsync($"PUT: api/sensor_data/{id} returned NotFound during concurrency check.");
                    return NotFound();
                }
                else
                {
                    await _logger.LogAsync($"PUT: api/sensor_data/{id} encountered a concurrency exception.");
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE api/sensor_data/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSensorData(int id)
        {
            await _logger.LogAsync($"DELETE: api/sensor_data/{id} called.");

            var sensorData = await _context.sensor_data.FindAsync(id);
            if (sensorData == null)
            {
                await _logger.LogAsync($"DELETE: api/sensor_data/{id} returned NotFound.");
                return NotFound();
            }

            _context.sensor_data.Remove(sensorData);
            await _context.SaveChangesAsync();

            await _logger.LogAsync($"DELETE: api/sensor_data/{id} deleted successfully.");
            return NoContent();
        }

        private bool SensorDataExists(int id)
        {
            return _context.sensor_data.Any(e => e.id == id);
        }
    }
}
