namespace aairos.Dto
{
    public class UserDeviceDto
    {
        public int userDeviceId { get; set; }
        public int profileId { get; set; }
        public int sensor_dataId { get; set; }
        public string? deviceStatus { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime updatedDate { get; set; }
    }
}
