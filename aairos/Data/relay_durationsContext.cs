using aairos.Dto;
using Microsoft.EntityFrameworkCore;

namespace aairos.Data
{
    public class relay_durationsContext : DbContext
    {
        public relay_durationsContext(DbContextOptions<relay_durationsContext> options)
           : base(options)
        {
        }

        public DbSet<aairos.Model.relay_durations> relay_durations { get; set; }
        public DbSet<aairos.Dto.StateDurationDTO> StateDurationDTO { get; set; }
    }
}
