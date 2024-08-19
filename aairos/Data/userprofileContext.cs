using Microsoft.EntityFrameworkCore;


namespace aairos.Data
{
    public class userprofileContext : DbContext
    {
        public userprofileContext (DbContextOptions<userprofileContext> options)
            : base(options)
        {
        }

        public DbSet<aairos.Model.userprofile> UserProfile { get; set; }
    }
}
