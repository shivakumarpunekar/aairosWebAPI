using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace aairos.Model
{
    public class userprofile
    {
        [Key]
        public int ProfileID { get; set; }
        public string ProfileGUID { get; set; } = Guid.NewGuid().ToString();
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string MobileNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public int PinCode { get; set; }
        public string EmailID { get; set; }
        public int NumberOfDevice { get; set; }
        public int LoginId { get; set; }

    }
}
