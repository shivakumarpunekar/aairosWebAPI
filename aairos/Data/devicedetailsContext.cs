using Microsoft.EntityFrameworkCore;
using aairos.Model;

namespace aairos.Data
{
    public class devicedetailsContext : DbContext
    {
        public devicedetailsContext(DbContextOptions<devicedetailsContext> options)
            : base(options)
        {
        }

        public DbSet<devicedetails> devicedetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<devicedetails>()
                .HasKey(d => d.DeviceDetailsID);
        }
    }
}
