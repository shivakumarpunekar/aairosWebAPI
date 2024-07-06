using aairos.Migrations.device;


namespace aairos.Model
{
    public class CombinedDataViewModel
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public string UserName { get; set; }
        public string MobileNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public int SensorId { get; set; }
        public int ValveId { get; set; }
        public int ValveStatus { get; set; }
    }
}
