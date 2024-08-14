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
            var top100PerDevice = await _context.sensor_data
             .FromSqlRaw(@"
                SELECT * FROM (
                  SELECT
                    *,
                    DENSE_RANK() OVER (PARTITION BY deviceId ORDER BY id DESC) AS r
                  FROM sensor_data
                ) AS t
                WHERE t.r <= 100 order by 1 desc")
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




        // GET: api/sensor_data/profile/{userProfileId}/device/{deviceId}
        [HttpGet("profile/{userProfileId}/device/{deviceId}")]
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
                .OrderByDescending(s => s.timestamp)
                .Take(100)
                .Select(s => new SensorDataDto
                {
                    id = s.id,
                    deviceId = s.deviceId,
                    sensor1_value = s.sensor1_value,
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



        // GET: api/sensor_data/device/{deviceId}/sensor2
        [HttpGet("device/{deviceId}/sensor2")]
        public async Task<ActionResult<IEnumerable<SensorDataDto>>> GetSensor2DataByDeviceId(int deviceId)
        {
            var data = await _context.sensor_data
                .Where(s => s.deviceId == deviceId)
                .OrderByDescending(s => s.timestamp)
                .Take(100)
                .Select(s => new SensorDataDto
                {
                    id = s.id,
                    deviceId = s.deviceId,
                    sensor2_value = s.sensor2_value,
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

        //This is based on date for 30 days
        // GET: api/uniqueDatesLast30Days
        [HttpGet("device/{deviceId}/uniqueDatesLast30Days")]
        public async Task<ActionResult<IEnumerable<string>>> GetUniqueCreatedDatesByDeviceIdLast30Days(int deviceId)
        {
            try
            {
                var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
                var data = await _context.sensor_data
                    .Where(s => s.deviceId == deviceId && s.solenoidValveStatus == true)
                    .Select(s => s.createdDateTime.Distinct())
                    .ToListAsync();

                /*var uniqueDates = data
                    .Select(dateString => DateTime.TryParse(dateString, out var createdDateTime) ? createdDateTime.Date : default(DateTime))
                    .Where(date => date != default(DateTime) && date >= thirtyDaysAgo)
                    .Distinct()
                    .OrderByDescending(date => date)
                    *//*.Select(date => date.ToString("yyyy-MM-dd")) // Format the date as yyyy-MM-dd*//*
                    .Select(date => date.ToString("dd-MM-yyyy"))
                    .ToList();*/

                if (!data.Any())
                {
                    return NotFound();
                }

                return Ok(data);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }


        // GET: api/sensor_data/date/{date}/device/{deviceId}
        [HttpGet("date/{date}/device/{deviceId}")]
        public async Task<ActionResult<IEnumerable<SensorDataDto>>> GetSensorDataByDate(string date, int deviceId)
        {
            if (!DateTime.TryParseExact(date, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
            {
                return BadRequest("Invalid date format. Please use dd-MM-yyyy format.");
            }

            var startOfDay = parsedDate.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

            var allData = await _context.sensor_data
                .Where(s => s.deviceId == deviceId && s.solenoidValveStatus == true)
                .ToListAsync();

            var data = allData
                .Where(s =>
                {
                    DateTime createdDateTime;
                    return DateTime.TryParse(s.createdDateTime, out createdDateTime) &&
                           createdDateTime >= startOfDay && createdDateTime <= endOfDay;
                })
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

            if (!data.Any())
            {
                return NotFound();
            }

            return Ok(data);
        }




        private bool SensorDataExists(int id)
        {
            return _context.sensor_data.Any(e => e.id == id);
        }
    }
}
