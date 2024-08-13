namespace aairos.Dto
{
    public class UserDeviceDto
    {
        public int userDeviceId { get; set; }
        public int userProfileId { get; set; }
        public int deviceId { get; set; }
        public string deviceStatus { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime updatedDate { get; set; }
    }
}
