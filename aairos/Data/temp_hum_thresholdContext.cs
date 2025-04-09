using Microsoft.EntityFrameworkCore;

namespace aairos.Data
{
    public class temp_hum_thresholdContext :DbContext
    {
        public temp_hum_thresholdContext(DbContextOptions<temp_hum_thresholdContext> options)
          : base(options)
        {
        }
        public DbSet<aairos.Model.temp_hum_thresholdModel> temp_hum_thresholdModel { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<aairos.Model.temp_hum_thresholdModel>()
                .ToTable("temp_hum_threshold")
                .Property(e => e.sensor_type)
                .HasConversion<string>();

            modelBuilder.Entity<aairos.Model.temp_hum_thresholdModel>()
                .Property(e => e.severity)
                .HasConversion<string>();
        }
    }
}
