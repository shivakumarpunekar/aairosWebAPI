using aairos.Data;
using aairos.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Security.Claims;
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

       
        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<IEnumerable<ValveStatus>>> GetValveStatusByDevice(int deviceId)
        {
            // Find the valve statuses by deviceId
            var valveStatuses = await _context.ValveStatus
                .Where(v => v.deviceId == deviceId)
                .ToListAsync();

            // If no valve statuses found, insert a new record
            if (valveStatuses == null || valveStatuses.Count == 0)
            {
                // Create a new ValveStatus entry
                var newValveStatus = new ValveStatus
                {
                    deviceId = deviceId,
                    ValveStatusOnOrOff = 0, // Default value, you can change this as needed
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    // Add other necessary properties like userProfileId if needed
                };

                // Add the new entry to the database
                _context.ValveStatus.Add(newValveStatus);
                await _context.SaveChangesAsync();

                // Fetch the newly inserted entry to return
                valveStatuses = new List<ValveStatus> { newValveStatus };
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



        // GET: api/ValveStatus/admin/device/{deviceId}
        [HttpGet("admin/device/{deviceId}")]
        public async Task<ActionResult> GetAdminValveStatusByDevice(int deviceId)
        {
            // Find the valve status by deviceId
            var valveStatus = await _context.ValveStatus
                .FirstOrDefaultAsync(v => v.deviceId == deviceId);

            if (valveStatus == null)
            {
                return NotFound("No matching valve status found for the given deviceId.");
            }

            // Return both AdminValveStatus and ValveStatusOnOrOff
            return Ok(new
            {
                valveStatus.AdminValveStatus,
                valveStatus.ValveStatusOnOrOff
            });
        }

        // PUT: api/ValveStatus/admin/device/{deviceId}
        [HttpPut("admin/device/{deviceId}")]
        public async Task<IActionResult> PutAdminValveStatusByDevice(int deviceId, ValveStatus valveStatus)
        {
            // Ensure AdminValveStatus is either 0, 1, or 2
            if (valveStatus.AdminValveStatus < 0 || valveStatus.AdminValveStatus > 2)
            {
                return BadRequest("AdminValveStatus must be 0, 1, or 2.");
            }

            // Find the existing status by deviceId
            var existingStatus = await _context.ValveStatus.FirstOrDefaultAsync(v => v.deviceId == deviceId);
            if (existingStatus == null)
            {
                return NotFound("No matching valve status found for the given deviceId.");
            }

            // Update AdminValveStatus
            existingStatus.AdminValveStatus = valveStatus.AdminValveStatus;
            existingStatus.UpdatedDate = DateTime.UtcNow;

            // Logic to update ValveStatusOnOrOff based on AdminValveStatus
            switch (valveStatus.AdminValveStatus)
            {
                case 0: // Off
                    existingStatus.ValveStatusOnOrOff = 0;
                    break;
                case 1: // On
                    existingStatus.ValveStatusOnOrOff = 1;
                    break;
                case 2: // Auto - retain current ValveStatusOnOrOff
                    break;
            }

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
