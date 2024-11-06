using aairos.Data;
using aairos.Dto;
using aairos.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceStateThresholdController : ControllerBase
    {
        private readonly relay_durationsContext _relayDurationsContext;
        private readonly ThresholdContext _thresholdContext;
        private readonly ValveStatusContext _valveStatusContext;
        private readonly sensor_dataContext _sensorDataContext;

        public DeviceStateThresholdController(
            relay_durationsContext relayDurationsContext,
            ThresholdContext thresholdContext,
            ValveStatusContext valveStatusContext,
            sensor_dataContext sensorDataContext)
        {
            _relayDurationsContext = relayDurationsContext;
            _thresholdContext = thresholdContext;
            _valveStatusContext = valveStatusContext;
            _sensorDataContext = sensorDataContext;
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

            // Fetch the valve status from ValveStatus table by deviceId
            var valveStatus = await _valveStatusContext.ValveStatus
                .FirstOrDefaultAsync(v => v.deviceId == deviceId);

            // Fetch the sensor data from sensordara table by deviceId
            var sensorData = await _sensorDataContext.sensor_data
               .Where(s => s.deviceId == deviceId)
               .OrderByDescending(s => s.id)
               .FirstOrDefaultAsync();

            if (relayDuration == null || threshold == null || valveStatus == null || sensorData == null)
            {
                return NotFound("Data not found for the given deviceId.");
            }

            // Set State based on ValveStatusOnOrOff (0 = Off, 1 = On)
            string state = valveStatus.ValveStatusOnOrOff == 0 ? "Off" : "On";

            // Map solenoidValveStatus to a bool (1 = true, 0 = false)
            bool solenoidValveStatus = sensorData.solenoidValveStatus;

            // Combine the data into the DTO
            var result = new DeviceStateThresholdDTO
            {
                State = state, // State is derived from ValveStatusOnOrOff
                Threshold_1 = threshold.Threshold_1,
                Threshold_2 = threshold.Threshold_2,
                AdminValveStatus = valveStatus.AdminValveStatus, // Add AdminValveStatus
                ValveStatusOnOrOff = valveStatus.ValveStatusOnOrOff, // Add ValveStatusOnOrOff
                solenoidValveStatus = sensorData.solenoidValveStatus ? "On" : "Off"
            };

            return Ok(result);
        }
    }
}
