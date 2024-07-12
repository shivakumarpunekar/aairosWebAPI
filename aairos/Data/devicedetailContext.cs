using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using aairos.Model;

namespace aairos.Data
{
    public class devicedetailContext : DbContext
    {
        public devicedetailContext (DbContextOptions<devicedetailContext> options)
            : base(options)
        {
        }

        public DbSet<aairos.Model.devicedetail> devicedetail { get; set; } 
    }
}
