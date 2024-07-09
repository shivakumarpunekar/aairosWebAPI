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
        private readonly devicedetailContext _devicedetailContext;
        private readonly userprofileContext _userProfileContext;
        public CombinedDataController(deviceContext deviceContext, devicedetailContext devicedetailContext, userprofileContext userProfileContext)
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
                var deviceDetails = await _devicedetailContext.devicedetail.ToListAsync();
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
                        SolonoidVale = detail.SolonoidVale,
                        ValveId = detail.ValveId,
                        ValveStatus = detail.ValveStatus
                    };
                }).ToList();

                return Ok(combinedData);
            }
            catch (Exception ex)
            {
                // Log the exception (implement logging as per your logging framework)
                Console.WriteLine(ex.Message);

                // Return a generic error message
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
                    Id = combinedData.DeviceId,
                    CreatedDate = combinedData.CreatedDate
                };

                var userProfileId = (await _userProfileContext.UserProfile.FirstOrDefaultAsync(u => u.UserName == combinedData.UserName))?.UserProfileId;

                if (!userProfileId.HasValue)
                {
                    return BadRequest("User profile not found");
                }

                var deviceDetail = new aairos.Model.devicedetail
                {
                    DeviceId = combinedData.DeviceId,
                    UserProfileId = userProfileId.Value,
                    Sensor_1 = combinedData.Sensor_1,
                    Sensor_2 = combinedData.Sensor_2,
                    SolonoidVale = combinedData.SolonoidVale,
                };

                var userProfile = new aairos.Model.userprofile
                {
                    UserProfileId = userProfileId.Value,
                    UserName = combinedData.UserName,
                    MobileNumber = combinedData.MobileNumber
                };

                // Add data to the respective contexts
                _deviceContext.device.Add(device);
                _devicedetailContext.devicedetail.Add(deviceDetail);
                _userProfileContext.UserProfile.Add(userProfile);

                // Save changes to the database
                await _deviceContext.SaveChangesAsync();
                await _devicedetailContext.SaveChangesAsync();
                await _userProfileContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception (implement logging as per your logging framework)
                Console.WriteLine(ex.Message);

                // Return a generic error message
                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }
}
