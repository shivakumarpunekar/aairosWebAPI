namespace aairos.Model
{
    public class ValveStatus
    {
        public int ValveStatusId { get; set; }
        public int ValveStatusOnOrOff { get; set; }
        public int deviceId { get; set; }
        public int userProfileId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
