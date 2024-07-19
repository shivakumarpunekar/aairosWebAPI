namespace aairos.Model
{
    public class Login
    {
        public int LoginId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public bool IsAdmin { get; set; }
        // Foreign key
        public int UserProfileId { get; set; }
    }
}
