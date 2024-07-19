using aairos.Data;
using aairos.Dto;
using aairos.Model;
using aairos.Services;
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
        private readonly UserDeviceContext _context;

        public UserDeviceController(UserDeviceContext context)
        {
            _context = context;
        }

        // GET: api/userdevice
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDevice>>> GetUserDevices()
        {
            var userDevices = await _context.UserDevice
                .Include(ud => ud.userprofile)
                .Include(ud => ud.sensor_data)
                .ToListAsync();

            return Ok(userDevices);
        }

        // GET api/userdevice/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDeviceDto>> GetUserDevice(int id)
        {
            var userDevice = await _context.UserDevice
                .Include(ud => ud.userprofile)
                .Include(ud => ud.sensor_data)
                .FirstOrDefaultAsync(ud => ud.userDeviceId == id);

            if (userDevice == null)
            {
                return NotFound();
            }

            // Update the UpdatedDate field
            userDevice.updatedDate = DateTime.UtcNow;

            _context.Entry(userDevice).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var userDeviceDto = new UserDeviceDto
            {
                userDeviceId = userDevice.userDeviceId,
                profileId = userDevice.profileId,
                sensor_dataId = userDevice.sensor_dataId,
                deviceStatus = userDevice.deviceStatus ? "On" : "Off",  // Convert bool to "On" or "Off"
                createdDate = userDevice.createdDate,
                updatedDate = userDevice.updatedDate
            };

            return Ok(userDeviceDto);
        }


        // POST api/userdevice
        [HttpPost]
        [HttpPost]
        public async Task<ActionResult<UserDeviceDto>> PostUserDevice([FromBody] UserDevice value)
        {
            value.createdDate = DateTime.UtcNow;
            value.updatedDate = DateTime.UtcNow;

            _context.UserDevice.Add(value);
            await _context.SaveChangesAsync();

            var userDeviceDto = new UserDeviceDto
            {
                userDeviceId = value.userDeviceId,
                profileId = value.profileId,
                sensor_dataId = value.sensor_dataId,
                deviceStatus = value.deviceStatus ? "On" : "Off",  // Convert bool to "On" or "Off"
                createdDate = value.createdDate,
                updatedDate = value.updatedDate
            };

            return CreatedAtAction(nameof(GetUserDevice), new { id = value.userDeviceId }, userDeviceDto);
        }

        // PUT api/userdevice/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserDevice(int id, [FromBody] UserDevice value)
        {
            if (id != value.userDeviceId)
            {
                return BadRequest();
            }

            value.updatedDate = DateTime.UtcNow;  // Ensure updatedDate is set

            _context.Entry(value).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.UserDevice.Any(e => e.userDeviceId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE api/userdevice/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserDevice(int id)
        {
            var userDevice = await _context.UserDevice.FindAsync(id);
            if (userDevice == null)
            {
                return NotFound();
            }

            _context.UserDevice.Remove(userDevice);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

