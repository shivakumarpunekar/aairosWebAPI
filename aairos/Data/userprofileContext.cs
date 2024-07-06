using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using aairos.Model;

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
