using aairos.Model;
using Microsoft.EntityFrameworkCore;

namespace aairos.Data
{
    public class sensor_dataContext : DbContext
    {
        public sensor_dataContext(DbContextOptions<sensor_dataContext> options)
           : base(options)
        {
        }

        public DbSet<aairos.Model.sensor_data> sensor_data { get; set; }
        public DbSet<aairos.Model.UserDevice> UserDevice { get; set; }
    }
}
