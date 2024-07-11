using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aairos.Data;
using aairos.Model;
using Microsoft.Extensions.Logging;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class devicedetailsController : ControllerBase
    {
        private readonly devicedetailsContext _context;
        private readonly ILogger<devicedetailsController> _logger;

        public devicedetailsController(devicedetailsContext context, ILogger<devicedetailsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/devicedetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<devicedetails>>> Getdevicedetail()
        {
            return await _context.devicedetails.ToListAsync();
        }

        //This is a GUID GET Method
        [HttpGet("byGuId/{guId}")]
        public async Task<ActionResult<devicedetails>> GetDeviceDetailByGuId(string guId)
        {
            var devicedetail = await _context.devicedetails.FirstOrDefaultAsync(d => d.DeviceDetailsGUID == guId);

            if (devicedetail == null)
            {
                return NotFound();
            }

            return devicedetail;
        }

        // GET: api/devicedetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<devicedetails>> Getdevicedetail(int id)
        {
            var devicedetail = await _context.devicedetails.FindAsync(id);

            if (devicedetail == null)
            {
                return NotFound();
            }

            return devicedetail;
        }

        // GET: api/devicedetails/bydeviceId/5
        [HttpGet("byId/{Id}")]
        public async Task<ActionResult<devicedetails>> GetdevicedetailById(int Id)
        {
            var devicedetail = await _context.devicedetails.FirstOrDefaultAsync(d => d.DeviceDetailsID == Id);

            if (devicedetail == null)
            {
                return NotFound();
            }

            return devicedetail;
        }

        // PUT: api/devicedetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putdevicedetail(int id, devicedetails devicedetail)
        {
            if (id != devicedetail.DeviceDetailsID)
            {
                return BadRequest();
            }

            _context.Entry(devicedetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!devicedetailExists(id))
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

        // POST: api/devicedetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<devicedetails>> Postdevicedetail(devicedetails devicedetail)
        {
            _context.devicedetails.Add(devicedetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Getdevicedetail), new { id = devicedetail.DeviceDetailsID }, devicedetail);
        }

        // DELETE: api/devicedetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletedevicedetail(int id)
        {
            var devicedetail = await _context.devicedetails.FindAsync(id);
            if (devicedetail == null)
            {
                return NotFound();
            }

            _context.devicedetails.Remove(devicedetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool devicedetailExists(int id)
        {
            return _context.devicedetails.Any(e => e.DeviceDetailsID == id);
        }
    }
}
