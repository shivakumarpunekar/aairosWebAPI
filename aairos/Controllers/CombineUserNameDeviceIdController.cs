using aairos.Data;
using aairos.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CombineUserNameDeviceIdController : ControllerBase
    {
        private readonly userprofileContext _userProfileContext;
        private readonly sensor_dataContext _sensor_dataContext;

        public CombineUserNameDeviceIdController(userprofileContext userProfileContext, sensor_dataContext sensor_dataContext)
        {
            _userProfileContext = userProfileContext;
            _sensor_dataContext = sensor_dataContext;
        }


        // GET: api/<CombineUserNameDeviceIdController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> Getuserprofile()
        {
            var userprofiles = await _userProfileContext.UserProfile
                .Select(up => new
                {
                    up.UserProfileId,
                    up.FirstName,
                    up.MiddleName,
                    up.LastName
                })
                .ToListAsync();

            return Ok(userprofiles);
        }

        // GET: api/<CombineUserNameDeviceIdController>/devices
        [HttpGet("devices")]
        public async Task<ActionResult<IEnumerable<object>>> GetDeviceIds()
        {
            var deviceIds = await _sensor_dataContext.sensor_data
            .Select(sensor_data => new
            {
                sensor_data.deviceId
            })
            .ToListAsync();

            return Ok(deviceIds);
        }
    }
}
