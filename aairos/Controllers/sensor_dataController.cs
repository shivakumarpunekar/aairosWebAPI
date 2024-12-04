using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aairos.Data;
using aairos.Model;
using aairos.Dto;
using aairos.Services;
using OfficeOpenXml;
using System.IO;
using MySqlConnector;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Google.Protobuf.WellKnownTypes;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class sensor_dataController : ControllerBase
    {
        
        private readonly sensor_dataContext _context;

        public sensor_dataController(sensor_dataContext context, FileLoggerService logger)
        {
            _context = context;
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
                return NotFound();
            }
            return Ok(data);
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
                return NotFound();
            }
            return Ok(sensorData);
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

            return CreatedAtAction(nameof(GetSensorData), new { id = value.id }, sensorDataDto);
        }

        // PUT api/sensor_data/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSensorData(int id, [FromBody] sensor_data value)
        {
            if (id != value.id)
            {
                return BadRequest();
            }

            _context.Entry(value).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SensorDataExists(id))
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

        // DELETE api/sensor_data/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSensorData(int id)
        {
            var sensorData = await _context.sensor_data.FindAsync(id);
            if (sensorData == null)
            {
                return NotFound();
            }

            _context.sensor_data.Remove(sensorData);
            await _context.SaveChangesAsync();

            return NoContent();
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

                // Define the SQL query to retrieve necessary data
                string query = @"
            SELECT 
                sd.Id AS Id,
                CONCAT(up.FirstName, ' ', up.MiddleName, ' ', up.LastName) AS Username,
                sd.deviceId AS DeviceID,
                sd.sensor1_value AS sensor1_value,
                sd.sensor2_value AS sensor2_value,
                CASE 
                    WHEN sd.solenoidValveStatus = 1 THEN 'On' 
                    ELSE 'Off' 
                END AS SolenoidValveStatus,
                DATE_FORMAT(sd.timestamp, '%Y-%m-%d %H:%i:%s') AS CreatedDateTime
            FROM 
                sensor_data sd
            JOIN 
                UserDevice ud ON sd.deviceId = ud.deviceId
            JOIN 
                UserProfile up ON ud.userProfileId = up.userProfileId
            WHERE 
                ud.userProfileId = @userProfileId AND
                sd.deviceId = @deviceId AND
                sd.timestamp >= @startDate AND
                sd.timestamp <= @endDate";

                // Use MySqlParameter for parameterized queries
                var data = await _context.sensor_data
                    .FromSqlRaw(query,
                        new MySqlConnector.MySqlParameter("@userProfileId", userProfileId),
                        new MySqlConnector.MySqlParameter("@deviceId", deviceId),
                        new MySqlConnector.MySqlParameter("@startDate", startDate),
                        new MySqlConnector.MySqlParameter("@endDate", endDate))
                    .Select(sd => new
                    {
                        Id = sd.id,
                        Username = sd.username,
                        DeviceID = sd.deviceId,
                        sensor1value = sd.sensor1_value,
                        sensor2value = sd.sensor2_value,
                        SolenoidValveStatus = sd.solenoidValveStatus ? "On" : "Off",
                        CreatedDateTime = sd.createdDateTime // This is now a string in the format yyyy-MM-dd HH:mm:ss
                    })
                    .ToListAsync();

                if (!data.Any())
                {
                    return NotFound(new { message = "No data found for the specified filters." });
                }

                // Stream the Excel file instead of loading it entirely into memory
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
                    worksheet.Cells[i + 2, 2].Value = data[i].DeviceID;
                    worksheet.Cells[i + 2, 3].Value = data[i].sensor1value;
                    worksheet.Cells[i + 2, 4].Value = data[i].sensor2value;
                    worksheet.Cells[i + 2, 5].Value = data[i].SolenoidValveStatus;
                    worksheet.Cells[i + 2, 6].Value = data[i].CreatedDateTime; // This is now a formatted string
                }

                // Format as table
                worksheet.Cells[1, 1, data.Count + 1, 6].AutoFitColumns();
                worksheet.Cells[1, 1, 1, 6].Style.Font.Bold = true;

                // Stream the result to the client
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
