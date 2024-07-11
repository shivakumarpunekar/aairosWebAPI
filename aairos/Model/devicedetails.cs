using System;
using System.ComponentModel.DataAnnotations;

namespace aairos.Model
{
    public class devicedetails
    {
        [Key]
        public int DeviceDetailsID { get; set; }
        public string DeviceDetailsGUID { get; set; } = Guid.NewGuid().ToString();
        public int DeviceID { get; set; }
        public int ProfileID { get; set; }
        public int? Sensor1 { get; set; }
        public int? ValveID { get; set; }
        public int? ValveStatus { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? Sensor2 { get; set; }
    }
}
