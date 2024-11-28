using Microsoft.EntityFrameworkCore;

namespace aairos.Data
{
    public class LoginContext : DbContext
    {
        public LoginContext(DbContextOptions<LoginContext> options)
            : base(options)
        {
        }

        public DbSet<aairos.Model.Login> Login { get; set; }
        public DbSet<aairos.Model.userprofile> userprofile { get; set; }
    }
}
