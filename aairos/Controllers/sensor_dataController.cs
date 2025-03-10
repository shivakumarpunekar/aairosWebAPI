using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aairos.Data;
using aairos.Model;
using aairos.Dto;
using aairos.Services;
using OfficeOpenXml;
using System.IO;

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
        public async Task<IActionResult> ExportToExcel([FromQuery] int userProfileId, [FromQuery] int deviceId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var sensorData = await (from sd in _context.sensor_data
                                    join ud in _context.UserDevice on sd.deviceId equals ud.deviceId
                                    join up in _context.UserProfile on ud.userProfileId equals up.userProfileId
                                    where ud.userProfileId == userProfileId && sd.deviceId == deviceId
                                    && sd.timestamp >= startDate && sd.timestamp <= endDate
                                    select new
                                    {
                                        Username = $"{up.FirstName} {up.MiddleName} {up.LastName}",
                                        sd.deviceId,
                                        sd.sensor1_value,
                                        sd.sensor2_value,
                                        solenoidValveStatus = sd.solenoidValveStatus ? "On" : "Off",
                                        sd.createdDateTime
                                    }).ToListAsync();

            if (!sensorData.Any())
            {
                return NotFound();
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sensor Data");
                worksheet.Cells["A1"].Value = "Username";
                worksheet.Cells["B1"].Value = "Device ID";
                worksheet.Cells["C1"].Value = "Sensor 1 Value";
                worksheet.Cells["D1"].Value = "Sensor 2 Value";
                worksheet.Cells["E1"].Value = "Solenoid Valve Status";
                worksheet.Cells["F1"].Value = "Created DateTime";

                var row = 2;
                foreach (var data in sensorData)
                {
                    worksheet.Cells[$"A{row}"].Value = data.Username;
                    worksheet.Cells[$"B{row}"].Value = data.deviceId;
                    worksheet.Cells[$"C{row}"].Value = data.sensor1_value;
                    worksheet.Cells[$"D{row}"].Value = data.sensor2_value;
                    worksheet.Cells[$"E{row}"].Value = data.solenoidValveStatus;
                    worksheet.Cells[$"F{row}"].Value = data.createdDateTime;
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;
                var fileName = $"SensorData_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
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
