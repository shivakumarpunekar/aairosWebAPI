using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using aairos.Model;
using aairos.Migrations.devicedetail;

namespace aairos.Data
{
    public class deviceContext : DbContext
    {
        public deviceContext (DbContextOptions<deviceContext> options)
            : base(options)
        {
        }

        public DbSet<aairos.Model.device> device { get; set; }
    }
}
