using Microsoft.AspNetCore.Mvc;
using aairos.Data;
using aairos.Model;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class historyvalvestatusController : ControllerBase
    {
        private readonly historyvalvestatusContext _context;

        public historyvalvestatusController(historyvalvestatusContext context)
        {
            _context = context;
        }

        // GET: api/ValveStatus
        [HttpGet]
        public async Task<ActionResult> GetHistoryValveStatus()
        {
            try
            {
                // Retrieve the data in descending order by UpdatedDate
                var data = await _context.historyvalvestatus
                                          .OrderByDescending(h => h.UpdatedDate)
                                          .ToListAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        // GET: api/historyvalvestatus/device/{deviceId}
        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult> GetHistoryValveStatusByDeviceId(int deviceId)
        {
            try
            {
                // Retrieve data for the specified deviceId in descending order by UpdatedDate
                var data = await _context.historyvalvestatus
                                          .Where(h => h.deviceId == deviceId)
                                          .OrderByDescending(h => h.UpdatedDate)
                                          .ToListAsync();

                if (data == null || data.Count == 0)
                {
                    return NotFound($"No history found for deviceId: {deviceId}");
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
    }
}
