using aairos.Migrations.device;

namespace aairos.Model
{
    public class devicedetail
    {
        public int DeviceDetailId { get; set; }
        // Foreign key
        public int Id { get; set; }
        public int LoginId { get; set; }
        public int userProfileId { get; set; }
        public string GuId { get; set; }
        public int DeviceId { get; set; }
        public int Sensor_1 { get; set; }
        public int Sensor_2 { get; set; }
        public int SolonoidVale {  get; set; }
        public int ValveId { get; set; }
        public int ValveStatus { get; set; }
    }
}
