using aairos.Data;
using aairos.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class temp_hum_thresholdController : ControllerBase
    {
        private readonly temp_hum_thresholdContext _context;

        public temp_hum_thresholdController(temp_hum_thresholdContext context)
        {
            _context = context;
        }

        // GET: api/T_H_Threshold
        [HttpGet]
        public async Task<ActionResult<IEnumerable<temp_hum_thresholdModel>>> GetAllThresholds()
        {
            return await _context.temp_hum_thresholdModel.ToListAsync();
        }

        // GET: api/T_H_Threshold/device/{device_id}
        [HttpGet("device/{device_id}")]
        public async Task<ActionResult<IEnumerable<temp_hum_thresholdModel>>> GetThresholdsByDeviceId(int device_id)
        {
            var thresholds = await _context.temp_hum_thresholdModel
                .Where(t => t.device_id == device_id)
                .ToListAsync();

            if (thresholds == null || thresholds.Count == 0)
            {
                return NotFound($"No thresholds found for device_id: {device_id}");
            }

            return thresholds;
        }

        // POST: api/T_H_Threshold
        [HttpPost]
        public async Task<ActionResult<temp_hum_thresholdModel>> CreateThreshold(temp_hum_thresholdModel model)
        {
            model.created_at = DateTime.UtcNow;

            _context.temp_hum_thresholdModel.Add(model);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetThresholdsByDeviceId), new { device_id = model.device_id }, model);
        }

        // PUT: api/T_H_Threshold/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateThreshold(int id, temp_hum_thresholdModel updatedModel)
        {
            if (id != updatedModel.id)
            {
                return BadRequest("ID mismatch");
            }

            var existingModel = await _context.temp_hum_thresholdModel.FindAsync(id);
            if (existingModel == null)
            {
                return NotFound($"Threshold with ID {id} not found.");
            }

            existingModel.device_id = updatedModel.device_id;
            existingModel.sensor_type = updatedModel.sensor_type;
            existingModel.severity = updatedModel.severity;

            _context.Entry(existingModel).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/T_H_Threshold/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteThreshold(int id)
        {
            var model = await _context.temp_hum_thresholdModel.FindAsync(id);
            if (model == null)
            {
                return NotFound($"Threshold with ID {id} not found.");
            }

            _context.temp_hum_thresholdModel.Remove(model);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
