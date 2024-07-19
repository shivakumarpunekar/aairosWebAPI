using aairos.Model;
using Microsoft.EntityFrameworkCore;

namespace aairos.Data
{
    public class UserDeviceContext : DbContext
    {
        public UserDeviceContext(DbContextOptions<UserDeviceContext> options)
           : base(options)
        {
        }

        public DbSet<aairos.Model.UserDevice> UserDevice { get; set; }
        public DbSet<aairos.Model.userprofile> userprofile { get; set; }
        public DbSet<aairos.Model.sensor_data> sensor_data { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDevice>()
                .HasOne(ud => ud.userprofile)
                .WithMany()
                .HasForeignKey(ud => ud.profileId);

            modelBuilder.Entity<UserDevice>()
                .HasOne(ud => ud.sensor_data)
                .WithMany()
                .HasForeignKey(ud => ud.sensor_dataId);
        }
    }
}