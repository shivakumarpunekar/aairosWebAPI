namespace aairos.DTO
{
    public class DeviceDto
    {
        public int Id { get; set; }
        public string GuId { get; set; }
        public int UserProfileId { get; set; }
        public int DeviceId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        // DeviceDetail properties
        public int DeviceDetailId { get; set; }
        public int SensorId { get; set; }
        public int ValveId { get; set; }
        public int ValveStatus { get; set; }
    }
}
