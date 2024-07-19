using aairos.Data;
using aairos.Dto;
using aairos.Model;
using aairos.Services;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Tls;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDeviceController : ControllerBase
    {
        private readonly UserDeviceContext _userdeviceContext;

        public UserDeviceController(UserDeviceContext context, FileLoggerService logger)
        {
            /*            _logger = logger;
            */
            _userdeviceContext = context;
        }

        // GET: api/<userdevicesController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDeviceDto>>> GetUserDevices()
        {
            var userDevices = await _userdeviceContext.UserDevice
                .ToListAsync();

            var userDeviceDtos = userDevices.Select(ud => new UserDeviceDto
            {
                userDeviceId = ud.userDeviceId,
                profileId = ud.profileId,
                sensor_dataId = ud.sensor_dataId,
                deviceStatus = ud.deviceStatus ? "Active" : "InActive",
                createdDate = ud.createdDate,
                updatedDate = ud.updatedDate,

            }).ToList();

            return Ok(userDeviceDtos);
        }


        // GET api/<userdevicesController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDeviceDto>> Getuserdevice(int id)
        {
            var userDevice = await _userdeviceContext.UserDevice
                .FirstOrDefaultAsync(ud => ud.userDeviceId == id);

            if (userDevice == null)
            {
                return NotFound();
            }

            var userDeviceDto = new UserDeviceDto
            {
                userDeviceId = userDevice.userDeviceId,
                profileId = userDevice.profileId,
                sensor_dataId = userDevice.sensor_dataId,
                deviceStatus = userDevice.deviceStatus ? "Active" : "InActive",
                createdDate = userDevice.createdDate,
                updatedDate = userDevice.updatedDate,
            };

            return Ok(userDeviceDto);
        }


        // POST api/<userdevicesController>
        [HttpPost]
        public async Task<ActionResult<UserDevice>> Postuserdevice([FromBody] UserDevice value)
        {
            _userdeviceContext.UserDevice.Add(value);
            await _userdeviceContext.SaveChangesAsync();

            var userDevicesDto = new UserDeviceDto
            {
                userDeviceId = value.userDeviceId,
                profileId = value.profileId,
                sensor_dataId = value.sensor_dataId,
                deviceStatus = value.deviceStatus ? "Active" : "InActive",
                createdDate = value.createdDate,
                updatedDate = value.updatedDate,
            };

/*            await _logger.LogAsync($"POST: api/userdevice created a new record with ID {value.id}.");
*/
            return CreatedAtAction(nameof(Getuserdevice), new { Id = value.userDeviceId }, userDevicesDto);
        }

        // PUT api/<userdevicesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Putusserdevice(int id, [FromBody] UserDevice value)
        {
            if (id != value.userDeviceId)
            {
/*                await _logger.LogAsync($"PUT: api/userdevice/{id} returned BadRequest due to ID mismatch.");
*/
                return BadRequest();
            }

            _userdeviceContext.Entry(value).State = EntityState.Modified;

            try
            {
                await _userdeviceContext.SaveChangesAsync();
/*                await _logger.LogAsync($"PUT: api/userdevice/{id} updated successfully.");
*/
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!userdeviceExists(id))
                {
/*                    await _logger.LogAsync($"PUT: api/userdevice/{id} returned NotFound during concurrency check.");
*/
                    return NotFound();
                }
                else
                {
/*                    await _logger.LogAsync($"PUT: api/userdevice/{id} encountered a concurrency exception.");
*/
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE api/<userdevicesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deleteuserdevices(int id)
        {
            var userdevice = await _userdeviceContext.UserDevice.FindAsync(id);
            if (userdevice == null)

            {
/*                await _logger.LogAsync($"DELETE: api/userdevices/{id} returned NotFound.");
*/
                return NotFound();
            }

            _userdeviceContext.UserDevice.Remove(userdevice);
            await _userdeviceContext.SaveChangesAsync();

/*            await _logger.LogAsync($"DELETE: api/userdevice/{id} deleted successfully.");
*/
            return NoContent();
        }

        private bool userdeviceExists(int id)
        {
            return _userdeviceContext.UserDevice.Any(e => e.userDeviceId == id);
        }
    }
}
        