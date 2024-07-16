using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aairos.Data;
using aairos.Model;
using aairos.Services;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CombinedDataController : ControllerBase
    {
/*        private readonly FileLoggerService _logger;
*/        private readonly deviceContext _deviceContext;
        private readonly devicedetailContext _deviceDetailContext;
        private readonly userprofileContext _userProfileContext;

        public CombinedDataController(deviceContext deviceContext, devicedetailContext deviceDetailContext, userprofileContext userProfileContext, FileLoggerService logger)
        {
/*            _logger = logger;
*/            _deviceContext = deviceContext;
            _deviceDetailContext = deviceDetailContext;
            _userProfileContext = userProfileContext;
        }

        // GET: api/CombinedData
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CombinedDataViewModel>>> GetCombinedData()
        {
            var devices = await _deviceContext.device.ToListAsync();
            var deviceDetails = await _deviceDetailContext.devicedetail.ToListAsync();
            var userProfiles = await _userProfileContext.UserProfile.ToListAsync();

            var combinedData = deviceDetails.Select(detail =>
            {
                var device = devices.FirstOrDefault(d => d.Id == detail.DeviceId);
                var userProfile = userProfiles.FirstOrDefault(u => u.UserProfileId == detail.UserProfileId);

                return new CombinedDataViewModel
                {
                    DeviceId = detail.DeviceId,
                    UserName = userProfile?.UserName,
                    MobileNumber = userProfile?.MobileNumber,
                    CreatedDate = device?.CreatedDate ?? default,
                    Sensor_1 = detail.Sensor_1,
                    Sensor_2 = detail.Sensor_2,
                    ValveId = detail.ValveId,
                    ValveStatus = detail.ValveStatus
                };
            }).ToList();

/*            await _logger.LogAsync($"GET: api/CombinedData returned {combinedData.Count} records.");
*/            return Ok(combinedData);
        }

        // POST: api/CombinedData
        [HttpPost]
        public async Task<ActionResult<CombinedDataViewModel>> CreateCombinedData([FromBody] CombinedDataViewModel inputData)
        {
            if (inputData == null)
            {
/*                await _logger.LogAsync("POST: api/CombinedData received null input data.");
*/                return BadRequest();
            }

            // Create a new UserProfile
            var userProfile = new userprofile
            {
                UserName = inputData.UserName,
                MobileNumber = inputData.MobileNumber
            };

            // Save the new UserProfile to get its ID
            _userProfileContext.UserProfile.Add(userProfile);
            await _userProfileContext.SaveChangesAsync();

            // Create a new Device
            var device = new device
            {
                CreatedDate = inputData.CreatedDate
            };

            // Save the new Device to get its ID
            _deviceContext.device.Add(device);
            await _deviceContext.SaveChangesAsync();

            // Create a new DeviceDetail
            var deviceDetail = new devicedetail
            {
                DeviceId = device.Id,
                UserProfileId = userProfile.UserProfileId,
                Sensor_1 = inputData.Sensor_1,
                Sensor_2 = inputData.Sensor_2,
                ValveId = inputData.ValveId,
                ValveStatus = inputData.ValveStatus
            };

            _deviceDetailContext.devicedetail.Add(deviceDetail);
            await _deviceDetailContext.SaveChangesAsync();

            var combinedData = new CombinedDataViewModel
            {
                DeviceId = deviceDetail.DeviceId,
                UserName = userProfile.UserName,
                MobileNumber = userProfile.MobileNumber,
                CreatedDate = device.CreatedDate,
                Sensor_1 = deviceDetail.Sensor_1,
                Sensor_2 = deviceDetail.Sensor_2,
                ValveId = deviceDetail.ValveId,
                ValveStatus = deviceDetail.ValveStatus
            };

/*            await _logger.LogAsync($"POST: api/CombinedData created new combined data with DeviceId {deviceDetail.DeviceId}.");
*/            return CreatedAtAction(nameof(GetCombinedData), new { id = deviceDetail.DeviceId }, combinedData);
        }
    }
}
