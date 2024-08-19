using aairos.Data;
using aairos.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValveStatusController : ControllerBase
    {
        private readonly ValveStatusContext _context;

        public ValveStatusController(ValveStatusContext context)
        {
            _context = context;
        }

        // GET: api/ValveStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ValveStatus>>> GetValveStatus()
        {
            return await _context.ValveStatus.ToListAsync();
        }

        // GET: api/ValveStatus/{userProfileId}/{deviceId}
        [HttpGet("{userProfileId}/{deviceId}")]
        public async Task<ActionResult<ValveStatus>> GetValveStatusByProfileAndDevice(int userProfileId, int deviceId)
        {
            // Find the valve status by userProfileId and deviceId
            var valveStatus = await _context.ValveStatus
                .FirstOrDefaultAsync(v => v.userProfileId == userProfileId && v.deviceId == deviceId);

            if (valveStatus == null)
            {
                return NotFound("No matching valve status found.");
            }

            // Return the valve status
            return Ok(valveStatus);
        }


        // POST: api/ValveStatus
        [HttpPost]
        public async Task<ActionResult<ValveStatus>> PostValveStatus(ValveStatus valveStatus)
        {
            // Ensure ValveStatusOnOrOff is either 0 or 1
            if (valveStatus.ValveStatusOnOrOff != 0 && valveStatus.ValveStatusOnOrOff != 1)
            {
                return BadRequest("ValveStatusOnOrOff must be 0 or 1.");
            }

            // Set CreatedDate for new entries
            valveStatus.CreatedDate = DateTime.UtcNow;
            valveStatus.UpdatedDate = DateTime.UtcNow;

            _context.ValveStatus.Add(valveStatus);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetValveStatus), new { id = valveStatus.ValveStatusId }, valveStatus);
        }

        // PUT: api/ValveStatus/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutValveStatus(int id, ValveStatus valveStatus)
        {
            if (id != valveStatus.ValveStatusId)
            {
                return BadRequest("ValveStatusId does not match.");
            }

            // Ensure ValveStatusOnOrOff is either 0 or 1
            if (valveStatus.ValveStatusOnOrOff != 0 && valveStatus.ValveStatusOnOrOff != 1)
            {
                return BadRequest("ValveStatusOnOrOff must be 0 or 1.");
            }

            var existingStatus = await _context.ValveStatus.FindAsync(id);
            if (existingStatus == null)
            {
                return NotFound();
            }

            // Update only the specified fields
            existingStatus.ValveStatusOnOrOff = valveStatus.ValveStatusOnOrOff;
            existingStatus.UpdatedDate = DateTime.UtcNow;

            _context.Entry(existingStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ValveStatusExists(id))
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

        // PUT: api/ValveStatus/{userProfileId}/{deviceId}
        [HttpPut("{userProfileId}/{deviceId}")]
        public async Task<IActionResult> PutValveStatus(int userProfileId, int deviceId, ValveStatus valveStatus)
        {
            // Ensure ValveStatusOnOrOff is either 0 or 1
            if (valveStatus.ValveStatusOnOrOff != 0 && valveStatus.ValveStatusOnOrOff != 1)
            {
                return BadRequest("ValveStatusOnOrOff must be 0 or 1.");
            }

            // Find the existing status by userProfileId and deviceId
            var existingStatus = await _context.ValveStatus
                .FirstOrDefaultAsync(v => v.userProfileId == userProfileId && v.deviceId == deviceId);

            if (existingStatus == null)
            {
                return NotFound("No matching valve status found.");
            }

            // Update only the specified fields
            existingStatus.ValveStatusOnOrOff = valveStatus.ValveStatusOnOrOff;
            existingStatus.UpdatedDate = DateTime.UtcNow;

            _context.Entry(existingStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ValveStatusExists(existingStatus.ValveStatusId))
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


        // GET: api/ValveStatus/device/{deviceId}
        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<IEnumerable<ValveStatus>>> GetValveStatusByDevice(int deviceId)
        {
            // Find the valve statuses by deviceId
            var valveStatuses = await _context.ValveStatus
                .Where(v => v.deviceId == deviceId)
                .ToListAsync();

            if (valveStatuses == null || valveStatuses.Count == 0)
            {
                return NotFound("No valve status found for the given deviceId.");
            }

            // Return the valve statuses
            return Ok(valveStatuses);
        }


        // PUT: api/ValveStatus/device/{deviceId}
        [HttpPut("device/{deviceId}")]
        public async Task<IActionResult> PutValveStatusByDevice(int deviceId, ValveStatus valveStatus)
        {
            // Ensure ValveStatusOnOrOff is either 0 or 1
            if (valveStatus.ValveStatusOnOrOff != 0 && valveStatus.ValveStatusOnOrOff != 1)
            {
                return BadRequest("ValveStatusOnOrOff must be 0 or 1.");
            }

            // Find the existing status by deviceId
            var existingStatus = await _context.ValveStatus
                .FirstOrDefaultAsync(v => v.deviceId == deviceId);

            if (existingStatus == null)
            {
                return NotFound("No matching valve status found for the given deviceId.");
            }

            // Update only the specified fields
            existingStatus.ValveStatusOnOrOff = valveStatus.ValveStatusOnOrOff;
            existingStatus.UpdatedDate = DateTime.UtcNow;

            _context.Entry(existingStatus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ValveStatusExists(existingStatus.ValveStatusId))
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



        private bool ValveStatusExists(int id)
        {
            return _context.ValveStatus.Any(e => e.ValveStatusId == id);
        }
    }
}
