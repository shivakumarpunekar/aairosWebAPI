namespace aairos.Model
{
    public class device
    {
        public int Id { get; set; }
        public int LoginId { get; set; }
        public Guid GuId { get; set; } = Guid.NewGuid();
        public int UserProfileId { get; set; }
        public int DeviceId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
