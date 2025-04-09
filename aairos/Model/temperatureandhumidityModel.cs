namespace aairos.Model
{
    public class temperatureandhumidityModel
    {
        public int id { get; set; }
        public int device_id { get; set; }
        public float temperature { get; set; }  // ENUM in DB
        public float humidity { get; set; }  // Default
        public DateTime created_at { get; set; }
    }
}
