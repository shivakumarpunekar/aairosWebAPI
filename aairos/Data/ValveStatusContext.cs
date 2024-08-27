using Microsoft.EntityFrameworkCore;

namespace aairos.Data
{
    public class ValveStatusContext : DbContext
    {
        public ValveStatusContext(DbContextOptions<ValveStatusContext> options)
            : base(options)
        {
        }

        public DbSet<aairos.Model.ValveStatus> ValveStatus { get; set; }

        public DbSet<aairos.Model.Login> Login { get; set; }

    }
}
