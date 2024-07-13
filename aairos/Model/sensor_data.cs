namespace aairos.Model
{
    public class sensor_data
    {
        public int id {  get; set; }
        public int sensor1_value { get; set; }
        public int sensor2_value { get; set; }
        public int deviceId { get; set; }
        public bool solenoidValveStatus { get; set; }
        public DateTime timestamp { get; set; }
    }
}
