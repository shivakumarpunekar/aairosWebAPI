using Microsoft.EntityFrameworkCore;

namespace aairos.Data
{
    public class temperatureandhumidityContext :DbContext
    {
        public temperatureandhumidityContext(DbContextOptions<temperatureandhumidityContext> options)
          : base(options)
        {
        }
        public DbSet<aairos.Model.temperatureandhumidityModel> temperatureandhumidityModel { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<aairos.Model.temperatureandhumidityModel>()
                .ToTable("temperatureandhumidity"); // ✅ map to actual table name
        }
    }
}
