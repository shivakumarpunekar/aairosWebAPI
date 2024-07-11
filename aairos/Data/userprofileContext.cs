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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure primary key
            modelBuilder.Entity<userprofile>()
                .HasKey(up => up.ProfileID);

            // Configure properties
            modelBuilder.Entity<userprofile>()
                .Property(up => up.ProfileID)
                .IsRequired();

            modelBuilder.Entity<userprofile>()
                .Property(up => up.ProfileGUID)
                .IsRequired()
                .HasMaxLength(36); // Assuming GUID is stored as string

            modelBuilder.Entity<userprofile>()
                .Property(up => up.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<userprofile>()
                .Property(up => up.MiddleName)
                .HasMaxLength(100);

            modelBuilder.Entity<userprofile>()
                .Property(up => up.LastName)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<userprofile>()
                .Property(up => up.DateOfBirth)
                .IsRequired();

            modelBuilder.Entity<userprofile>()
                .Property(up => up.MobileNumber)
                .IsRequired()
                .HasMaxLength(15);

            modelBuilder.Entity<userprofile>()
                .Property(up => up.UserName)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<userprofile>()
                .Property(up => up.Password)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<userprofile>()
                .Property(up => up.CreatedDate)
                .IsRequired();

            modelBuilder.Entity<userprofile>()
                .Property(up => up.UpdatedDate)
                .IsRequired();

            modelBuilder.Entity<userprofile>()
                .Property(up => up.Address)
                .HasMaxLength(200);

            modelBuilder.Entity<userprofile>()
                .Property(up => up.City)
                .HasMaxLength(100);

            modelBuilder.Entity<userprofile>()
                .Property(up => up.State)
                .HasMaxLength(100);

            modelBuilder.Entity<userprofile>()
                .Property(up => up.Country)
                .HasMaxLength(100);

            modelBuilder.Entity<userprofile>()
                .Property(up => up.PinCode)
                .IsRequired();

            modelBuilder.Entity<userprofile>()
                .Property(up => up.EmailID)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<userprofile>()
                .Property(up => up.NumberOfDevice)
                .IsRequired();

            modelBuilder.Entity<userprofile>()
                .Property(up => up.LoginId)
                .IsRequired();

            // Configure unique constraints
            modelBuilder.Entity<userprofile>()
                .HasIndex(up => up.UserName)
                .IsUnique();

            modelBuilder.Entity<userprofile>()
                .HasIndex(up => up.MobileNumber)
                .IsUnique();

            modelBuilder.Entity<userprofile>()
                .HasIndex(up => up.EmailID)
                .IsUnique();
        }
    }
}
