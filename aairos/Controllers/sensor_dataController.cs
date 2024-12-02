using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aairos.Data;
using aairos.Model;
using aairos.Dto;
using aairos.Services;
using OfficeOpenXml;
using System.IO;
using MySqlConnector;

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

        // GET: api/sensor_data/profile/{userProfileId}/device/{deviceId}
        /*[HttpGet("profile/{userProfileId}/device/{deviceId}")]
        public async Task<ActionResult<IEnumerable<SensorDataDto>>> GetSensorDataByuserProfileIdAndDeviceId(int userProfileId, int deviceId)
        {
            var data = await (from sd in _context.sensor_data
                              join ud in _context.UserDevice on sd.deviceId equals ud.deviceId
                              where ud.userProfileId == userProfileId && sd.deviceId == deviceId
                              orderby sd.timestamp descending
                              select new SensorDataDto
                              {
                                  id = sd.id,
                                  sensor1_value = sd.sensor1_value,
                                  sensor2_value = sd.sensor2_value,
                                  deviceId = sd.deviceId,
                                  solenoidValveStatus = sd.solenoidValveStatus ? "On" : "Off",
                                  timestamp = sd.timestamp,
                                  createdDateTime = sd.createdDateTime
                              }).ToListAsync();

            if (data == null || !data.Any())
            {
                return NotFound();
            }

            return Ok(data);
        }*/

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

        [HttpGet("export")]
        public async Task<IActionResult> DownloadExcel([FromQuery] int userProfileId, [FromQuery] int deviceId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                // Set the LicenseContext property for EPPlus
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Increase the command timeout (default is 30 seconds)
                _context.Database.SetCommandTimeout(180); // Set the timeout to 3 minutes

                // Fetch data from the database
                var data = await (from sd in _context.sensor_data.AsNoTracking()
                                  join ud in _context.UserDevice.AsNoTracking() on sd.deviceId equals ud.deviceId
                                  join up in _context.UserProfile.AsNoTracking() on ud.userProfileId equals up.userProfileId
                                  where ud.userProfileId == userProfileId
                                        && sd.deviceId == deviceId
                                        && sd.timestamp >= startDate
                                        && sd.timestamp <= endDate
                                  select new
                                  {
                                       sd.id,
                                      Username = $"{up.FirstName} {up.MiddleName} {up.LastName}".Trim(),
                                      sd.deviceId,
                                      sd.sensor1_value,
                                      sd.sensor2_value,
                                      SolenoidValveStatus = sd.solenoidValveStatus ? "On" : "Off",
                                      sd.timestamp
                                  }).ToListAsync();

                if (!data.Any())
                {
                    return NotFound(new { message = "No data found for the specified filters." });
                }

                // Generate Excel file
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Sensor Data");

                // Add headers
                worksheet.Cells[1, 1].Value = "Username";
                worksheet.Cells[1, 2].Value = "Device ID";
                worksheet.Cells[1, 3].Value = "Sensor1 Value";
                worksheet.Cells[1, 4].Value = "Sensor2 Value";
                worksheet.Cells[1, 5].Value = "Solenoid Valve Status";
                worksheet.Cells[1, 6].Value = "Created DateTime";

                // Populate data
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].Username;
                    worksheet.Cells[i + 2, 2].Value = data[i].deviceId;
                    worksheet.Cells[i + 2, 3].Value = data[i].sensor1_value;
                    worksheet.Cells[i + 2, 4].Value = data[i].sensor2_value;
                    worksheet.Cells[i + 2, 5].Value = data[i].SolenoidValveStatus;
                    worksheet.Cells[i + 2, 6].Value = data[i].timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                }

                // Format as table
                worksheet.Cells[1, 1, data.Count + 1, 6].AutoFitColumns();
                worksheet.Cells[1, 1, 1, 6].Style.Font.Bold = true;

                // Convert to a byte array
                var excelData = package.GetAsByteArray();

                // Return as a file download
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SensorData.xlsx");
            }
            catch (Exception ex)
            {
                // Log the error details (You may want to log this to a logging service or file)
                return StatusCode(500, new { message = "An error occurred while generating the Excel file.", error = ex.Message });
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
