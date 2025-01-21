using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aairos.Data;
using aairos.Model;
using aairos.Dto;
using aairos.Services;
using OfficeOpenXml;
using System.IO;
using System.Text;
using System.Globalization;
using System.Drawing.Printing;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class sensor_dataController : ControllerBase
    {
        // This is for logging.
/*        private readonly FileLoggerService _logger;
*/        private readonly sensor_dataContext _context;

        public sensor_dataController(sensor_dataContext context, FileLoggerService logger)
        {
/*            _logger = logger;
*/            _context = context;
        }

        // GET: api/sensor_data
        [HttpGet]
        public async Task<ActionResult<IEnumerable<sensor_data>>> GetSensorData()
        {
            var data = await _context.sensor_data
                .OrderByDescending(s => s.timestamp)
                .Take(100)
                .Select(s => new SensorDataDto
                {
                    id = s.id,
                    sensor1_value = s.sensor1_value,
                    sensor2_value = s.sensor2_value,
                    deviceId = s.deviceId,
                    solenoidValveStatus = s.solenoidValveStatus ? "On" : "Off",
                    timestamp = s.timestamp,
                    createdDateTime = s.createdDateTime,
                })
                .ToListAsync();

/*            await _logger.LogAsync($"GET: api/sensor_data returned {data.Count} records.");
*/
            return Ok(data);
        }

        // GET: api/sensor_data/top100perdevice
        [HttpGet("top100perdevice")]
        public async Task<ActionResult<IEnumerable<SensorDataDto>>> GetTop100SensorDataPerDevice()
        {
            try
            {
                var top100PerDevice = await _context.sensor_data
                    .FromSqlRaw(@"
                         SELECT sd.*
                         FROM sensor_data sd
                         INNER JOIN (
                             SELECT deviceId, MAX(id) AS max_id
                             FROM sensor_data
                             GROUP BY deviceId
                         ) AS latest ON sd.deviceId = latest.deviceId AND sd.id = latest.max_id
                         ORDER BY sd.id DESC")
                    .Select(s => new SensorDataDto
                    {
                        id = s.id,
                        sensor1_value = s.sensor1_value,
                        sensor2_value = s.sensor2_value,
                        deviceId = s.deviceId,
                        solenoidValveStatus = s.solenoidValveStatus ? "On" : "Off",
                        timestamp = s.timestamp,
                        createdDateTime = s.createdDateTime,
                    })
                    .ToListAsync();

                return Ok(top100PerDevice);
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here)
                return StatusCode(500, "Internal server error");
            }

        }

        // GET: api/GetUniqueDeviceIds
        [HttpGet("deviceId")]
        public async Task<ActionResult<IEnumerable<object>>> GetUniqueDeviceIds()
        {
            var uniqueDeviceIds = await _context.sensor_data
                .Select(s => s.deviceId)
                .Distinct()
                .ToListAsync();

            return Ok(uniqueDeviceIds);
        }

        // GET: api/sensor_data/device/{deviceId}
        [HttpGet("device/{deviceId}")]
        public async Task<ActionResult<IEnumerable<SensorDataDto>>> GetSensorDataByDeviceId(int deviceId)
        {
            var data = await _context.sensor_data
                .Where(s => s.deviceId == deviceId)
                .OrderByDescending(s => s.id)
                .Take(30)
                .Select(s => new SensorDataDto
                {
                    id = s.id,
                    sensor1_value = s.sensor1_value,
                    sensor2_value = s.sensor2_value,
                    deviceId = s.deviceId,
                    solenoidValveStatus = s.solenoidValveStatus ? "On" : "Off",
                    timestamp = s.timestamp,
                    createdDateTime = s.createdDateTime,
                })
                .ToListAsync();

            if (!data.Any())
            {
/*                await _logger.LogAsync($"GET: api/sensor_data/device/{deviceId} returned NotFound.");
*/                return NotFound();
            }

            /*            await _logger.LogAsync($"GET: api/sensor_data/device/{deviceId} returned {data.Count} records.");
            */            return Ok(data);
        }

        // GET api/sensor_data/5
        [HttpGet("{id}")]
        public async Task<ActionResult<sensor_data>> GetSensorData(int id)
        {
            var sensorData = await _context.sensor_data
                .Where(s => s.id == id)
                .Select(s => new SensorDataDto
                {
                    id = s.id,
                    sensor1_value = s.sensor1_value,
                    sensor2_value = s.sensor2_value,
                    deviceId = s.deviceId,
                    solenoidValveStatus = s.solenoidValveStatus ? "On" : "Off",
                    timestamp = s.timestamp,
                    createdDateTime = s.createdDateTime,
                })
                .FirstOrDefaultAsync(s => s.id == id);

            if (sensorData == null)
            {
/*                await _logger.LogAsync($"GET: api/sensor_data/{id} returned NotFound.");
*/                return NotFound();
            }

/*            await _logger.LogAsync($"GET: api/sensor_data/{id} returned a record.");
*/            return Ok(sensorData);
        }

        // POST api/sensor_data
        [HttpPost]
        public async Task<ActionResult<sensor_data>> PostSensorData([FromBody] sensor_data value)
        {
            _context.sensor_data.Add(value);
            await _context.SaveChangesAsync();

            var sensorDataDto = new SensorDataDto
            {
                id = value.id,
                sensor1_value = value.sensor1_value,
                sensor2_value = value.sensor2_value,
                deviceId = value.deviceId,
                solenoidValveStatus = value.solenoidValveStatus ? "On" : "Off",
                timestamp = value.timestamp,
                createdDateTime = value.createdDateTime,
            };

/*            await _logger.LogAsync($"POST: api/sensor_data created a new record with ID {value.id}.");
*/            return CreatedAtAction(nameof(GetSensorData), new { id = value.id }, sensorDataDto);
        }

        // PUT api/sensor_data/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSensorData(int id, [FromBody] sensor_data value)
        {
            if (id != value.id)
            {
/*                await _logger.LogAsync($"PUT: api/sensor_data/{id} returned BadRequest due to ID mismatch.");
*/                return BadRequest();
            }

            _context.Entry(value).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
/*                await _logger.LogAsync($"PUT: api/sensor_data/{id} updated successfully.");
*/            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SensorDataExists(id))
                {
/*                    await _logger.LogAsync($"PUT: api/sensor_data/{id} returned NotFound during concurrency check.");
*/                    return NotFound();
                }
                else
                {
/*                    await _logger.LogAsync($"PUT: api/sensor_data/{id} encountered a concurrency exception.");
*/                    throw;
                }
            }

            return NoContent();
        }

        // DELETE api/sensor_data/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSensorData(int id)
        {
            var sensorData = await _context.sensor_data.FindAsync(id);
            if (sensorData == null)
            {
/*                await _logger.LogAsync($"DELETE: api/sensor_data/{id} returned NotFound.");
*/                return NotFound();
            }

            _context.sensor_data.Remove(sensorData);
            await _context.SaveChangesAsync();

/*            await _logger.LogAsync($"DELETE: api/sensor_data/{id} deleted successfully.");
*/            return NoContent();
        }

        // GET: api/sensor_data/device/{deviceId}/sensor1
        [HttpGet("device/{deviceId}/sensor1")]
        public async Task<ActionResult<IEnumerable<SensorDataDto>>> GetSensor1DataByDeviceId(int deviceId)
        {
            var data = await _context.sensor_data
                .Where(s => s.deviceId == deviceId)
                .OrderByDescending(s => s.id)
                .Take(2)
                .Select(s => new SensorDataDto
                {
                    id = s.id,
                    deviceId = s.deviceId,
                    sensor1_value = s.sensor1_value,
                    timestamp = s.timestamp,
                })
                .ToListAsync();

            if (!data.Any())
            {
                return NotFound();
            }

            return Ok(data);
        }

        // GET: api/sensor_data/device/{deviceId}/sensor2
        [HttpGet("device/{deviceId}/sensor2")]
        public async Task<ActionResult<IEnumerable<SensorDataDto>>> GetSensor2DataByDeviceId(int deviceId)
        {
            var data = await _context.sensor_data
                .Where(s => s.deviceId == deviceId)
                .OrderByDescending(s => s.id)
                .Take(2)
                .Select(s => new SensorDataDto
                {
                    id = s.id,
                    deviceId = s.deviceId,
                    sensor2_value = s.sensor2_value,
                    timestamp = s.timestamp,
                })
                .ToListAsync();

            if (!data.Any())
            {
                return NotFound();
            }

            return Ok(data);
        }

        /*[HttpGet("export")]
        public async Task<IActionResult> ExportToCsv(int userProfileId, int deviceId, DateTime startDate, DateTime endDate)
        {
            var data = await _context.sensor_data
                .Join(_context.UserDevice, sd => sd.deviceId, ud => ud.deviceId, (sd, ud) => new { sd, ud })
                .Join(_context.UserProfile, combined => combined.ud.userProfileId, up => up.userProfileId, (combined, up) => new
                {
                    Username = up.FirstName + " " + (up.MiddleName ?? "") + " " + up.LastName,
                    combined.sd.deviceId,
                    combined.sd.sensor1_value,
                    combined.sd.sensor2_value,
                    SolenoidValveStatus = combined.sd.solenoidValveStatus ? "On" : "Off",
                    combined.sd.timestamp,
                    up.userProfileId
                })
                .Where(record => record.deviceId == deviceId && record.userProfileId == userProfileId)
                .Take(1000000)
                .ToListAsync();

            // Filter in-memory after fetching the data using timestamp
            var filteredData = data
                .Where(record =>
                    record.timestamp >= startDate && record.timestamp <= endDate)
                .ToList();

            var csv = new StringBuilder();
            csv.AppendLine("Username,DeviceID,Sensor1Value,Sensor2Value,SolenoidValveStatus,Timestamp");

            foreach (var row in filteredData)
            {
                var formattedDate = row.timestamp.ToString("dd-MM-yyyy HH:mm:ss");
                csv.AppendLine($"{row.Username},{row.deviceId},{row.sensor1_value},{row.sensor2_value},{row.SolenoidValveStatus},{formattedDate}");
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());

            // Include deviceId in the file name
            string fileName = $"sensor_data_{deviceId}.csv";

            return File(bytes, "text/csv", fileName);
        }*/

        [HttpGet("export")]
        public async Task<IActionResult> ExportToCsv(int userProfileId, int deviceId, DateTime startDate, DateTime endDate)
        {
            try
            {
                // Fetch relevant data
                var data = await _context.sensor_data
                    .Join(_context.UserDevice, sd => sd.deviceId, ud => ud.deviceId, (sd, ud) => new { sd, ud })
                    .Join(_context.UserProfile, combined => combined.ud.userProfileId, up => up.userProfileId, (combined, up) => new
                    {
                        Username = up.FirstName + " " + (up.MiddleName ?? "") + " " + up.LastName,
                        combined.sd.deviceId,
                        combined.sd.sensor1_value,
                        combined.sd.sensor2_value,
                        SolenoidValveStatus = combined.sd.solenoidValveStatus ? "On" : "Off",
                        combined.sd.timestamp,
                        up.userProfileId
                    })
                    .Where(record => record.deviceId == deviceId && record.userProfileId == userProfileId)
                    .ToListAsync();

                // Filter data within the provided date and time range
                var filteredData = data
                    .Where(record =>
                        record.timestamp >= startDate && record.timestamp <= endDate)
                    .OrderBy(record => record.timestamp) // Order by timestamp
                    .ToList();

                // Handle case where no data is found
                if (!filteredData.Any())
                {
                    return BadRequest("No data found for the specified filters.");
                }

                // Create CSV content
                var csv = new StringBuilder();
                csv.AppendLine("Username,DeviceID,Sensor1Value,Sensor2Value,SolenoidValveStatus,Timestamp");

                foreach (var row in filteredData)
                {
                    var formattedDate = row.timestamp.ToString("dd-MM-yyyy HH:mm:ss");
                    csv.AppendLine($"{row.Username},{row.deviceId},{row.sensor1_value},{row.sensor2_value},{row.SolenoidValveStatus},{formattedDate}");
                }

                // Convert CSV content to byte array
                var bytes = Encoding.UTF8.GetBytes(csv.ToString());

                // Include deviceId and timestamp in the file name for uniqueness
                string fileName = $"sensor_data_{deviceId}_{DateTime.Now:yyyyMMddHHmmss}.csv";

                // Return CSV file
                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                // Log error and return generic failure message
                Console.WriteLine($"Error generating CSV: {ex.Message}");
                return StatusCode(500, "An error occurred while generating the CSV file.");
            }
        }


        [HttpGet("device/{deviceId}/uniqueDatesLast30Days")]
        public async Task<ActionResult<IEnumerable<string>>> GetUniqueCreatedDatesByDeviceIdLast30Days(int deviceId)
        {
            try
            {
                // Declare and initialize the thirtyDaysAgo variable
                var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

                var data = await _context.sensor_data
                .Where(s => s.deviceId == deviceId && s.solenoidValveStatus == true)
                .Select(s => s.createdDateTime)
                .ToListAsync();

                Console.WriteLine("Retrieved data: " + string.Join(", ", data)); // Log retrieved data

                var uniqueDates = data
                    .Select(dateString => DateTime.TryParse(dateString, out var createdDateTime) ? createdDateTime : (DateTime?)null)
                    .Where(date => date != null && date.Value >= thirtyDaysAgo)
                    .Select(date => date.Value.Date)
                    .Distinct()
                    .OrderByDescending(date => date)
                    .Select(date => date.ToString("yyyy-MM-dd"))
                    .ToList();

                Console.WriteLine("Unique dates: " + string.Join(", ", uniqueDates)); // Log unique dates


                if (!uniqueDates.Any())
                {
                    return NotFound();
                }

                return Ok(uniqueDates);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }


        // GET: api/sensor_data/date/{date}/device/{deviceId}
        [HttpGet("date/{date}/device/{deviceId}")]
        public async Task<ActionResult<IEnumerable<SensorDataDto>>> GetSensorDataByDate(string date, int deviceId)
        {
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
            {
                return BadRequest("Invalid date format. Please use yyyy-MM-dd format.");
            }

            var startOfDay = parsedDate.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            var data = await _context.sensor_data
                .Where(s => s.deviceId == deviceId && s.solenoidValveStatus == true)
                .ToListAsync();

            var filteredData = data
                .Where(s => DateTime.TryParse(s.createdDateTime, out var createdDateTime)
                            && createdDateTime >= startOfDay && createdDateTime <= endOfDay)
                .OrderByDescending(s => s.timestamp)
                .Select(s => new SensorDataDto
                {
                    id = s.id,
                    sensor1_value = s.sensor1_value,
                    sensor2_value = s.sensor2_value,
                    deviceId = s.deviceId,
                    solenoidValveStatus = s.solenoidValveStatus ? "On" : "Off",
                    timestamp = s.timestamp,
                    createdDateTime = s.createdDateTime,
                })
                .ToList();

            if (!filteredData.Any())
            {
                return NotFound();
            }

            return Ok(filteredData);
        }

        private bool SensorDataExists(int id)
        {
            return _context.sensor_data.Any(e => e.id == id);
        }
    }
}
