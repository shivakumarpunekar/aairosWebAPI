﻿using aairos.Data;
using aairos.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aairos.Handular;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace aairos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThresholdController : ControllerBase
    {
        private readonly ThresholdContext _context;
        private readonly sensor_dataContext _sensorDataContext;

        public ThresholdController(ThresholdContext context, sensor_dataContext sensorDataContext)
        {
            _context = context;
            _sensorDataContext = sensorDataContext;
        }

        // GET: api/Threshold
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Threshold>>> GetAllThresholds()
        {
            var thresholds = await _context.Threshold
                                           .OrderByDescending(t => t.Id)
                                           .Take(100)
                                           .ToListAsync();
            return Ok(thresholds);
        }

        // POST: api/Threshold
        [HttpPost]
        public async Task<IActionResult> CreateThresholds(int userProfileId, int deviceId)
        {
            // Fetch the 100 most recent sensor data records
            var sensorDataList = await _sensorDataContext.sensor_data
                .Where(sd => sd.deviceId == deviceId)
                .OrderByDescending(sd => sd.createdDateTime)
                .Take(100)
                .ToListAsync();

            if (sensorDataList == null || !sensorDataList.Any())
            {
                return NotFound("No sensor data found for the specified device.");
            }

            var thresholds = new List<Threshold>();

            // Filter and loop through each sensor data record to create a corresponding threshold
            foreach (var sensorData in sensorDataList)
            {
                // Only create thresholds for values <= 1250 or >= 4000
                if ((sensorData.sensor1_value <= 1250 || sensorData.sensor1_value >= 4000) ||
                    (sensorData.sensor2_value <= 1250 || sensorData.sensor2_value >= 4000))
                {
                    var threshold = new Threshold
                    {
                        userProfileId = userProfileId,
                        deviceId = deviceId,
                        Threshold_1 = (sensorData.sensor1_value <= 1250 || sensorData.sensor1_value >= 4000) ? sensorData.sensor1_value : 0,
                        Threshold_2 = (sensorData.sensor2_value <= 1250 || sensorData.sensor2_value >= 4000) ? sensorData.sensor2_value : 0,
                        createdDateTime = sensorData.createdDateTime,
                        updatedDateTime = DateTime.UtcNow
                    };
                    thresholds.Add(threshold);
                }
            }

            if (!thresholds.Any())
            {
                return BadRequest("No valid sensor data found matching the threshold criteria.");
            }

            try
            {
                // Save all thresholds to the database
                _context.Threshold.AddRange(thresholds);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return CreatedAtAction(nameof(GetAllThresholds), thresholds);
        }



        // GET: api/Threshold/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Threshold>> GetThresholdById(int id)
        {
            var threshold = await _context.Threshold.FindAsync(id);

            if (threshold == null)
            {
                return NotFound();
            }

            return threshold;
        }


        // POST: api/Threshold/CreateSingle
        [HttpPost("CreateSingle")]
        public async Task<IActionResult> CreateSingleThreshold(int userProfileId, int deviceId, int sensor1_value, int sensor2_value)
        {
            // Fetch the 10 most recent sensor data records
            var sensorDataList = await _sensorDataContext.sensor_data
                .Where(sd => sd.deviceId == deviceId)
                .OrderByDescending(sd => sd.createdDateTime)
                .Take(10)
                .ToListAsync();

            if (sensorDataList == null || !sensorDataList.Any())
            {
                return NotFound("No sensor data found for the specified device.");
            }

            var thresholds = new List<Threshold>();

            // Filter and loop through each sensor data record to create a corresponding threshold
            foreach (var sensorData in sensorDataList)
            {
                // Check if the provided sensor1_value and sensor2_value trigger the threshold logic
                bool sensor1Trigger = (sensor1_value <= 1250 && sensorData.sensor1_value <= 1250) ||
                                      (sensor1_value >= 4000 && sensorData.sensor1_value >= 4000);

                bool sensor2Trigger = (sensor2_value <= 1250 && sensorData.sensor2_value <= 1250) ||
                                      (sensor2_value >= 4000 && sensorData.sensor2_value >= 4000);

                if (sensor1Trigger || sensor2Trigger)
                {
                    var threshold = new Threshold
                    {
                        userProfileId = userProfileId,
                        deviceId = deviceId,
                        Threshold_1 = sensor1Trigger ? sensorData.sensor1_value : 0,
                        Threshold_2 = sensor2Trigger ? sensorData.sensor2_value : 0,
                        createdDateTime = sensorData.createdDateTime,
                        updatedDateTime = DateTime.UtcNow
                    };
                    thresholds.Add(threshold);
                }
            }

            if (!thresholds.Any())
            {
                return BadRequest("No valid sensor data found matching the threshold criteria.");
            }

            try
            {
                // Save all thresholds to the database
                _context.Threshold.AddRange(thresholds);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return CreatedAtAction(nameof(GetAllThresholds), thresholds);
        }


    }
}
