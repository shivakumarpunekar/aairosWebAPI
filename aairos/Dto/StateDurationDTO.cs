namespace aairos.Dto
{
    public class StateDurationDTO
    {
        public int Id { get; set; }
        public int user_id { get; set; }
        public DateTime Day { get; set; }
        public int TotalDurationOnMinutes { get; set; }
        public int TotalDurationOffMinutes { get; set; }
    }
}
