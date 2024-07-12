﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using aairos.Data;

#nullable disable

namespace aairos.Migrations.devicedetail
{
    [DbContext(typeof(devicedetailContext))]
    partial class devicedetailContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("aairos.Model.devicedetail", b =>
                {
                    b.Property<int>("DeviceDetailId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("DeviceDetailId"));

                    b.Property<int>("DeviceId")
                        .HasColumnType("int");

                    b.Property<string>("GuId")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("SensorId")
                        .HasColumnType("int");

                    b.Property<int>("UserProfileId")
                        .HasColumnType("int");

                    b.Property<int>("ValveId")
                        .HasColumnType("int");

                    b.Property<int>("ValveStatus")
                        .HasColumnType("int");

                    b.HasKey("DeviceDetailId");

                    b.ToTable("devicedetail");
                });
#pragma warning restore 612, 618
        }
    }
}
