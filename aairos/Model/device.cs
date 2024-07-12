namespace aairos.Model
{
    public class device
    {
        public int Id { get; set; }
        public int LoginId { get; set; }
        public string GuId { get; set; }
        public int UserProfileId { get; set; }
        public int DeviceId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        // Navigation property for userprofile
        public virtual userprofile UserProfile { get; set; }
    }
}
