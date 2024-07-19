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
    }
}