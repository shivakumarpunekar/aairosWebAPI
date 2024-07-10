namespace aairos.Model
{
    public class device
    {
        public int DeviceID { get; set; }
        public string DeviceGUID { get; set; } = Guid.NewGuid().ToString();
        public int UserProfileID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
