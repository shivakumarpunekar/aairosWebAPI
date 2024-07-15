using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aairos.Data;
using aairos.Model;
using aairos.Services;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class devicesController : ControllerBase
    {
        private readonly FileLoggerService _logger;
        private readonly deviceContext _context;

        public devicesController(deviceContext context, FileLoggerService logger)
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/devices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<device>>> Getdevice()
        {
            var devices = await _context.device.ToListAsync();
            await _logger.LogAsync($"GET: api/devices returned {devices.Count} records.");
            return devices;
        }

        // GET: api/devices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<device>> Getdevice(int id)
        {
            var device = await _context.device.FindAsync(id);

            if (device == null)
            {
                await _logger.LogAsync($"GET: api/devices/{id} returned NotFound.");
                return NotFound();
            }

            await _logger.LogAsync($"GET: api/devices/{id} returned a device.");
            return device;
        }

        // PUT: api/devices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Putdevice(int id, device device)
        {
            if (id != device.Id)
            {
                await _logger.LogAsync($"PUT: api/devices/{id} returned BadRequest due to ID mismatch.");
                return BadRequest();
            }

            _context.Entry(device).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await _logger.LogAsync($"PUT: api/devices/{id} updated successfully.");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!deviceExists(id))
                {
                    await _logger.LogAsync($"PUT: api/devices/{id} returned NotFound during concurrency check.");
                    return NotFound();
                }
                else
                {
                    await _logger.LogAsync($"PUT: api/devices/{id} encountered a concurrency exception.");
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
            _context.device.Add(device);
            await _context.SaveChangesAsync();

            await _logger.LogAsync($"POST: api/devices created a new device with ID {device.Id}.");
            return CreatedAtAction("Getdevice", new { id = device.Id }, device);
        }

        // DELETE: api/devices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletedevice(int id)
        {
            var device = await _context.device.FindAsync(id);
            if (device == null)
            {
                await _logger.LogAsync($"DELETE: api/devices/{id} returned NotFound.");
                return NotFound();
            }

            _context.device.Remove(device);
            await _context.SaveChangesAsync();

            await _logger.LogAsync($"DELETE: api/devices/{id} deleted successfully.");
            return NoContent();
        }

        private bool deviceExists(int id)
        {
            return _context.device.Any(e => e.Id == id);
        }
    }
}
