namespace aairos.Model
{
    public enum SensorType
    {
        temperature,
        humidity
    }

    public enum Severity
    {
        warning,
        critical
    }
    public class temp_hum_thresholdModel
    {
        public int id { get; set; }
        public int device_id { get; set; }
        public SensorType sensor_type { get; set; }  // ENUM in DB
        public Severity severity { get; set; } = Severity.warning;  // Default
        public DateTime created_at { get; set; }
    }
}
