using aairos.Data;
using aairos.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceStateThresholdController : ControllerBase
    {
        private readonly relay_durationsContext _relayDurationsContext;
        private readonly ThresholdContext _thresholdContext;

        public DeviceStateThresholdController(relay_durationsContext relayDurationsContext, ThresholdContext thresholdContext)
        {
            _relayDurationsContext = relayDurationsContext;
            _thresholdContext = thresholdContext;
        }

        // GET: api/Device/{deviceId}
        [HttpGet("{deviceId}")]
        public async Task<ActionResult<DeviceStateThresholdDTO>> GetDeviceStateAndThresholds(int deviceId)
        {
            // Fetch the latest state from relay_durations by deviceId
            var relayDuration = await _relayDurationsContext.relay_durations
                .Where(r => r.user_id == deviceId)
                .OrderByDescending(r => r.timestamp) // Assuming you want the most recent state
                .FirstOrDefaultAsync();

            // Fetch the thresholds from Threshold table by deviceId
            var threshold = await _thresholdContext.Threshold
                .FirstOrDefaultAsync(t => t.deviceId == deviceId);

            if (relayDuration == null || threshold == null)
            {
                return NotFound("Data not found for the given deviceId.");
            }

            // Combine the data into the DTO
            var result = new DeviceStateThresholdDTO
            {
                State = relayDuration.state,
                Threshold_1 = threshold.Threshold_1,
                Threshold_2 = threshold.Threshold_2
            };

            return Ok(result);
        }
    }
}
