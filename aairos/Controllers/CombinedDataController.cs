using aairos.Data;
using aairos.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CombinedDataController : ControllerBase
    {
        private readonly deviceContext _deviceContext;
        private readonly devicedetailsContext _devicedetailContext;
        private readonly userprofileContext _userProfileContext;
        public CombinedDataController(deviceContext deviceContext, devicedetailsContext devicedetailContext, userprofileContext userProfileContext)
        {
            _deviceContext = deviceContext;
            _devicedetailContext = devicedetailContext;
            _userProfileContext = userProfileContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CombinedDataViewModel>>> GetCombinedData()
        {
            try
            {
                var devices = await _deviceContext.device.ToListAsync();
                var deviceDetails = await _devicedetailContext.devicedetails.ToListAsync();
                var userProfiles = await _userProfileContext.UserProfile.ToListAsync();

                var combinedData = deviceDetails.Select(detail =>
                {
                    var device = devices.FirstOrDefault(d => d.DeviceID == detail.DeviceID);
                    var userProfile = userProfiles.FirstOrDefault(u => u.ProfileID == detail.DeviceID);

                    return new CombinedDataViewModel
                    {
                        DeviceId = detail.DeviceID,
                        UserName = userProfile?.UserName,
                        MobileNumber = userProfile?.MobileNumber,
                        CreatedDate = device?.CreatedDate ?? default,
                        Sensor_1 = detail.Sensor1,
                        Sensor_2 = detail.Sensor2,
                        ValveId = detail.ValveID,
                        ValveStatus = detail.ValveStatus
                    };
                }).ToList();

                return Ok(combinedData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return StatusCode(500, "An internal server error occurred.");
            }
        }

        [HttpPost]
        public async Task<ActionResult> PostCombinedData([FromBody] CombinedDataViewModel combinedData)
        {
            try
            {
                var device = new aairos.Model.device
                {
                    // Assuming Device class has the same properties
                    DeviceID = combinedData.DeviceId,
                    CreatedDate = combinedData.CreatedDate
                };

                var ProfileID = (await _userProfileContext.UserProfile.FirstOrDefaultAsync(u => u.UserName == combinedData.UserName))?.ProfileID;

                if (!ProfileID.HasValue)
                {
                    return BadRequest("User profile not found");
                }

                var deviceDetail = new aairos.Model.devicedetails
                {
                    DeviceID = combinedData.DeviceId,
                    ProfileID = ProfileID.Value,
                    Sensor1 = combinedData.Sensor_1,
                    Sensor2 = combinedData.Sensor_2,
                };

                var userProfile = new aairos.Model.userprofile
                {
                    ProfileID = ProfileID.Value,
                    UserName = combinedData.UserName,
                    MobileNumber = combinedData.MobileNumber
                };

                // Add data to the respective contexts
                _deviceContext.device.Add(device);
                _devicedetailContext.devicedetails.Add(deviceDetail);
                _userProfileContext.UserProfile.Add(userProfile);

                // Save changes to the database
                await _deviceContext.SaveChangesAsync();
                await _devicedetailContext.SaveChangesAsync();
                await _userProfileContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }
}
