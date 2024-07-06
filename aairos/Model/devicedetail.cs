using aairos.Migrations.device;

namespace aairos.Model
{
    public class devicedetail
    {
        public int DeviceDetailId { get; set; }
        public string GuId { get; set; } = Guid.NewGuid().ToString();
        public int DeviceId { get; set; }
        public int SensorId { get; set; }
        public int ValveId { get; set; }
        public int UserProfileId { get; set; }
        public int ValveStatus { get; set; }
        // Foreign key
        public int Id { get; set; }
    }
}
