using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aairos.Data;
using aairos.Model;
using Microsoft.AspNetCore.Authorization;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class devicedetailsController : ControllerBase
    {
        private readonly devicedetailsContext _context;

        public devicedetailsController(devicedetailsContext context)
        {
            _context = context;
        }

        // GET: api/devicedetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<devicedetails>>> Getdevicedetail()
        {
            return await _context.devicedetail.ToListAsync();
        }


        //This is a guId GET Method
        [HttpGet("byGuId/{guId}")]
        public async Task<ActionResult<devicedetails>> GetDeviceDetailByGuId(string guId)
        {
            var devicedetail = await _context.devicedetail.FirstOrDefaultAsync(d => d.DeviceDetailsGUID == guId);

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
            var devicedetail = await _context.devicedetail.FindAsync(id);

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
            var devicedetail = await _context.devicedetail.FirstOrDefaultAsync(d => d.DeviceDetailsID == Id);

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
            _context.devicedetail.Add(devicedetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Getdevicedetail", new { id = devicedetail.DeviceDetailsID }, devicedetail);
        }

        // DELETE: api/devicedetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletedevicedetail(int id)
        {
            var devicedetail = await _context.devicedetail.FindAsync(id);
            if (devicedetail == null)
            {
                return NotFound();
            }

            _context.devicedetail.Remove(devicedetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool devicedetailExists(int id)
        {
            return _context.devicedetail.Any(e => e.DeviceDetailsID == id);
        }
    }
}
