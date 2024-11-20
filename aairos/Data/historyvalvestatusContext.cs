using aairos.Model;
using Microsoft.EntityFrameworkCore;

namespace aairos.Data
{
    public class historyvalvestatusContext : DbContext
    {
        public historyvalvestatusContext(DbContextOptions<historyvalvestatusContext> options)
            : base(options)
        {
        }

        public DbSet<historyvalvestatusModel> historyvalvestatus { get; set; }

        public DbSet<aairos.Model.Login> Login { get; set; }
    }
}
