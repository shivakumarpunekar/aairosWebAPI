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

    }
}
