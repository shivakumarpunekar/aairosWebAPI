using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aairos.Data;
using aairos.Model;
using aairos.Dto;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class sensor_dataController : ControllerBase
    {
        private readonly sensor_dataContext _context;

        public sensor_dataController(sensor_dataContext context)
        {
            _context = context;
        }

        // GET: api/sensor_data
        [HttpGet]
        public async Task<ActionResult<IEnumerable<sensor_data>>> GetSensorData()
        {
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

            return Ok(data);
        }

        // GET api/sensor_data/5
        [HttpGet("{id}")]
        public async Task<ActionResult<sensor_data>> GetSensorData(int id)
        {
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
                return NotFound();
            }

            return Ok(sensorData);
        }

        // POST api/sensor_data
        [HttpPost]
        public async Task<ActionResult<sensor_data>> PostSensorData([FromBody] sensor_data value)
        {
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

            return CreatedAtAction(nameof(GetSensorData), new { id = value.id }, value);
        }

        // PUT api/sensor_data/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSensorData(int id, [FromBody] sensor_data value)
        {
            if (id != value.id)
            {
                return BadRequest();
            }

            _context.Entry(value).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SensorDataExists(id))
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

        // DELETE api/sensor_data/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSensorData(int id)
        {
            var sensorData = await _context.sensor_data.FindAsync(id);
            if (sensorData == null)
            {
                return NotFound();
            }

            _context.sensor_data.Remove(sensorData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SensorDataExists(int id)
        {
            return _context.sensor_data.Any(e => e.id == id);
        }
    }
}
