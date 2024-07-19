namespace aairos.Model
{
    public class UserDevice
    {
        public int userDeviceId { get; set; }
        public int profileId { get; set;}
        public int sensor_dataId { get; set; }
        public bool deviceStatus { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime updatedDate { get; set; }

        // Navigation properties
        public virtual userprofile? userprofile { get; set; }
        public virtual sensor_data? sensor_data { get; set; }
    }
}
