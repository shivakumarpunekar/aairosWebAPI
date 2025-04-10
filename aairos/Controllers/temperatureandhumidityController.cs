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
            DateTime twoHoursAgo = DateTime.Now.AddHours(-2);

            var thresholds = await _context.temperatureandhumidityModel
                .Where(t => t.device_id == device_id && t.created_at >= twoHoursAgo)
                .OrderByDescending(t => t.created_at)
                .ToListAsync();

            if (thresholds == null || thresholds.Count == 0)
            {
                return NotFound($"No data found for device_id: {device_id}");
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
                return NotFound($"data with ID {id} not found.");
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
                return NotFound($"data with ID {id} not found.");
            }

            _context.temperatureandhumidityModel.Remove(model);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/T_H_Threshold/device/{device_id}/download?startDate=2025-04-09&endDate=2025-04-10
        [HttpGet("device/{device_id}/download")]
        public async Task<IActionResult> DownloadThresholdCsv(int device_id, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            // Fix start to 00:00 and end to 23:59:59
            DateTime fromDate = startDate.Date; // 00:00:00
            DateTime toDate = endDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59); // 23:59:59

            var thresholds = await _context.temperatureandhumidityModel
                .Where(t => t.device_id == device_id && t.created_at >= fromDate && t.created_at <= toDate)
                .OrderByDescending(t => t.created_at)
                .ToListAsync();

            if (thresholds == null || thresholds.Count == 0)
            {
                return NotFound($"No data found for device_id: {device_id} between {fromDate:yyyy-MM-dd HH:mm} and {toDate:yyyy-MM-dd HH:mm}");
            }

            var csv = new System.Text.StringBuilder();
            csv.AppendLine("id,device_id,temperature,humidity,created_at");

            foreach (var t in thresholds)
            {
                csv.AppendLine($"{t.id},{t.device_id},{t.temperature} °C,{t.humidity} %,{t.created_at:yyyy-MM-dd HH:mm:ss}");
            }

            var fileName = $"temperatureandhumidity_device_{device_id}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            var fileBytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());

            return File(fileBytes, "text/csv", fileName);
        }

    }
}
