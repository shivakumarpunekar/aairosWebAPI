namespace aairos.Model
{
    public class UserDevice
    {
        public int userDeviceId { get; set; }
        public int profileId { get; set;}
        public int deviceId { get; set; }
        public bool deviceStatus { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime updatedDate { get; set; }
    }
}
