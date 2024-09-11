namespace aairos.Dto
{
    public class SensorDataDto
    {
            public int id { get; set; }
            public int sensor1_value { get; set; }
            public int sensor2_value { get; set; }
            public int deviceId { get; set; }
            public string? solenoidValveStatus { get; set; }
            public DateTime timestamp { get; set; }
            public string? createdDateTime { get; set; }
    }
}
