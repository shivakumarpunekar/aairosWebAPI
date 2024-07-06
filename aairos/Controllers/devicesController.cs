using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aairos.Data;
using aairos.Model;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class devicesController : ControllerBase
    {
        private readonly deviceContext _context;

        public devicesController(deviceContext context)
        {
            _context = context;
        }

        // GET: api/devices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<device>>> Getdevice()
        {
            return await _context.device.ToListAsync();
        }


        //This is a guId GET Method
        [HttpGet("byGuId/{guId}")]
        public async Task<ActionResult<device>> GetDeviceByGuId(string guId)
        {
            var device = await _context.device.FirstOrDefaultAsync(d => d.GuId == guId);

            if (device == null)
            {
                return NotFound();
            }

            return device;
        }

        // GET: api/devices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<device>> Getdevice(int id)
        {
            var device = await _context.device.FindAsync(id);

            if (device == null)
            {
                return NotFound();
            }

            return device;
        }

        // PUT: api/devices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putdevice(int id, device device)
        {
            if (id != device.Id)
            {
                return BadRequest();
            }

            device.UpdatedDate = DateTime.UtcNow;
            _context.Entry(device).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!deviceExists(id))
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

        // POST: api/devices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<device>> Postdevice(device device)
        {
            device.CreatedDate = DateTime.UtcNow;
            _context.device.Add(device);
            await _context.SaveChangesAsync();

            return CreatedAtAction("Getdevice", new { id = device.Id }, device);
        }

        // DELETE: api/devices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletedevice(int id)
        {
            var device = await _context.device.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }

            _context.device.Remove(device);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool deviceExists(int id)
        {
            return _context.device.Any(e => e.Id == id);
        }
    }
}
