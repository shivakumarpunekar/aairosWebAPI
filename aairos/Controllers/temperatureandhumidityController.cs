using aairos.Data;
using aairos.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class temperatureandhumidityController : ControllerBase
    {
        private readonly temperatureandhumidityContext _context;

        public temperatureandhumidityController(temperatureandhumidityContext context)
        {
            _context = context;
        }

        // GET: api/T_H_Threshold
        [HttpGet]
        public async Task<ActionResult<IEnumerable<temperatureandhumidityModel>>> GetAllThresholds()
        {
            return await _context.temperatureandhumidityModel
                .OrderByDescending(t => t.created_at)
                .ToListAsync();
        }

        // GET: api/T_H_Threshold/device/{device_id}
        [HttpGet("device/{device_id}")]
        public async Task<ActionResult<IEnumerable<temperatureandhumidityModel>>> GetThresholdsByDeviceId(int device_id)
        {
            var thresholds = await _context.temperatureandhumidityModel
                .Where(t => t.device_id == device_id)
                .OrderByDescending(t => t.created_at)
                .ToListAsync();

            if (thresholds == null || thresholds.Count == 0)
            {
                return NotFound($"No thresholds found for device_id: {device_id}");
            }

            return thresholds;
        }

        // POST: api/T_H_Threshold
        [HttpPost]
        public async Task<ActionResult<temperatureandhumidityModel>> CreateThreshold(temperatureandhumidityModel model)
        {
            model.created_at = DateTime.UtcNow;

            _context.temperatureandhumidityModel.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetThresholdsByDeviceId), new { device_id = model.device_id }, model);
        }

        // PUT: api/T_H_Threshold/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateThreshold(int id, temperatureandhumidityModel updatedModel)
        {
            if (id != updatedModel.id)
            {
                return BadRequest("ID mismatch");
            }

            var existingModel = await _context.temperatureandhumidityModel.FindAsync(id);
            if (existingModel == null)
            {
                return NotFound($"Threshold with ID {id} not found.");
            }

            existingModel.device_id = updatedModel.device_id;
            existingModel.temperature = updatedModel.temperature;
            existingModel.humidity = updatedModel.humidity;

            _context.Entry(existingModel).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/T_H_Threshold/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteThreshold(int id)
        {
            var model = await _context.temperatureandhumidityModel.FindAsync(id);
            if (model == null)
            {
                return NotFound($"Threshold with ID {id} not found.");
            }

            _context.temperatureandhumidityModel.Remove(model);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
