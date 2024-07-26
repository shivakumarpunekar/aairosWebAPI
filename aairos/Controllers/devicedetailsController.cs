using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aairos.Data;
using aairos.Model;
using aairos.Services;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class devicedetailsController : ControllerBase
    {
/*        private readonly FileLoggerService _logger;
*/        private readonly devicedetailContext _context;

        public devicedetailsController(devicedetailContext context, FileLoggerService logger)
        {
/*            _logger = logger;
*/            _context = context;
        }

        // GET: api/devicedetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<devicedetail>>> Getdevicedetail()
        {
            /*return await  _context.devicedetail.ToListAsync();*/
            var devicedetail = await _context.devicedetail.ToListAsync();
/*            await _logger.LogAsync($"GET: api/devicedetails returned {devicedetail.Count} records.");
*/            return Ok(devicedetail);
        }

        // GET: api/devicedetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<devicedetail>> Getdevicedetail(int id)
        {
            var devicedetail = await _context.devicedetail.FindAsync(id);

            if (devicedetail == null)
            {
/*                await _logger.LogAsync($"GET: api/devicedetails/{id} returned NotFound.");
*/                return NotFound();
            }

/*            await _logger.LogAsync($"GET: api/devicedetails/{id} returned a record.");
*/            return Ok(devicedetail);
        }

        // GET: api/devicedetails/bydeviceId/5
        [HttpGet("byId/{Id}")]
        public async Task<ActionResult<devicedetail>> GetdevicedetailById(int Id)
        {
            var devicedetail = await _context.devicedetail.FirstOrDefaultAsync(d => d.Id == Id);

            if (devicedetail == null)
            {
/*                await _logger.LogAsync($"GET: api/devicedetails/byId/{Id} returned NotFound.");
*/                return NotFound();
            }

/*            await _logger.LogAsync($"GET: api/devicedetails/byId/{Id} returned a record.");
*/            return Ok(devicedetail);
        }

        // PUT: api/devicedetails/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Putdevicedetail(int id, devicedetail devicedetail)
        {
            if (id != devicedetail.DeviceDetailId)
            {
/*                await _logger.LogAsync($"PUT: api/devicedetails/{id} returned BadRequest due to ID mismatch.");
*/                return BadRequest();
            }

            _context.Entry(devicedetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
/*                await _logger.LogAsync($"PUT: api/devicedetails/{id} updated successfully.");
*/            }
            catch (DbUpdateConcurrencyException)
            {
                if (!devicedetailExists(id))
                {
/*                    await _logger.LogAsync($"PUT: api/devicedetails/{id} returned NotFound during concurrency check.");
*/                    return NotFound();
                }
                else
                {
/*                    await _logger.LogAsync($"PUT: api/devicedetails/{id} encountered a concurrency exception.");
*/                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/devicedetails
        [HttpPost]
        public async Task<ActionResult<devicedetail>> Postdevicedetail(devicedetail devicedetail)
        {
            _context.devicedetail.Add(devicedetail);
            await _context.SaveChangesAsync();

/*            await _logger.LogAsync($"POST: api/devicedetails created a new record with ID {devicedetail.DeviceDetailId}.");
*/            return CreatedAtAction("Getdevicedetail", new { id = devicedetail.DeviceDetailId }, devicedetail);
        }

        // DELETE: api/devicedetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletedevicedetail(int id)
        {
            var devicedetail = await _context.devicedetail.FindAsync(id);
            if (devicedetail == null)
            {
/*                await _logger.LogAsync($"DELETE: api/devicedetails/{id} returned NotFound.");
*/                return NotFound();
            }

            _context.devicedetail.Remove(devicedetail);
            await _context.SaveChangesAsync();

/*            await _logger.LogAsync($"DELETE: api/devicedetails/{id} deleted successfully.");
*/            return NoContent();
        }

        private bool devicedetailExists(int id)
        {
            return _context.devicedetail.Any(e => e.DeviceDetailId == id);
        }
    }
}
