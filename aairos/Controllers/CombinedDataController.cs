using aairos.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CombinedDataController : ControllerBase
    {
        private readonly deviceContext _deviceContext;
        private readonly devicedetailContext _deviceDetailContext;
        private readonly userprofileContext _userProfileContext;
        public CombinedDataController(deviceContext deviceContext, devicedetailContext deviceDetailContext, userprofileContext userProfileContext)
        {
            _deviceContext = deviceContext;
            _deviceDetailContext = deviceDetailContext;
            _userProfileContext = userProfileContext;
        }

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
                    SensorId = detail.SensorId,
                    ValveId = detail.ValveId,
                    ValveStatus = detail.ValveStatus
                };
            }).ToList();

            return Ok(combinedData);
        }
    }
}
