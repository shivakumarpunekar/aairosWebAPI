using Microsoft.EntityFrameworkCore;

namespace aairos.Data
{
    public class ThresholdContext : DbContext
    {
        public ThresholdContext(DbContextOptions<ThresholdContext> options)
           : base(options)
        {
        }

        public DbSet<aairos.Model.Threshold> Threshold { get; set; }
    }
}
