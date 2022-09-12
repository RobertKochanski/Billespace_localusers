﻿// <auto-generated />
using System;
using BilleSpace.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BilleSpace.Infrastructure.Migrations
{
    [DbContext(typeof(BilleSpaceDbContext))]
    [Migration("20220831095735_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("BilleSpace.Infrastructure.Entities.City", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CountryId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CountryId");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("BilleSpace.Infrastructure.Entities.Country", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Symbol")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("BilleSpace.Infrastructure.Entities.Office", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AuthorNameIdentifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("CityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("OfficeMapUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CityId");

                    b.ToTable("Offices");
                });

            modelBuilder.Entity("BilleSpace.Infrastructure.Entities.OfficeZone", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Desks")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("OfficeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OfficeId");

                    b.ToTable("OfficeZones");
                });

            modelBuilder.Entity("BilleSpace.Infrastructure.Entities.ParkingZone", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("OfficeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Spaces")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OfficeId");

                    b.ToTable("ParkingZones");
                });

            modelBuilder.Entity("BilleSpace.Infrastructure.Entities.Receptionist", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserNameIdentifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Receptionists");
                });

            modelBuilder.Entity("BilleSpace.Infrastructure.Entities.Reservation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("OfficeDesk")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OfficeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OfficeZoneId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ParkingSpace")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ParkingZoneId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("UserNameIdentifier")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OfficeId");

                    b.HasIndex("OfficeZoneId");

                    b.HasIndex("ParkingZoneId");

                    b.ToTable("Reservations");
                });

            modelBuilder.Entity("BilleSpace.Infrastructure.Entities.City", b =>
                {
                    b.HasOne("BilleSpace.Infrastructure.Entities.Country", "Country")
                        .WithMany()
                        .HasForeignKey("CountryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Country");
                });

            modelBuilder.Entity("BilleSpace.Infrastructure.Entities.Office", b =>
                {
                    b.HasOne("BilleSpace.Infrastructure.Entities.City", "City")
                        .WithMany()
                        .HasForeignKey("CityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("City");
                });

            modelBuilder.Entity("BilleSpace.Infrastructure.Entities.OfficeZone", b =>
                {
                    b.HasOne("BilleSpace.Infrastructure.Entities.Office", null)
                        .WithMany("OfficeZones")
                        .HasForeignKey("OfficeId");
                });

            modelBuilder.Entity("BilleSpace.Infrastructure.Entities.ParkingZone", b =>
                {
                    b.HasOne("BilleSpace.Infrastructure.Entities.Office", null)
                        .WithMany("ParkingZones")
                        .HasForeignKey("OfficeId");
                });

            modelBuilder.Entity("BilleSpace.Infrastructure.Entities.Reservation", b =>
                {
                    b.HasOne("BilleSpace.Infrastructure.Entities.Office", "Office")
                        .WithMany()
                        .HasForeignKey("OfficeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BilleSpace.Infrastructure.Entities.OfficeZone", "OfficeZone")
                        .WithMany()
                        .HasForeignKey("OfficeZoneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BilleSpace.Infrastructure.Entities.ParkingZone", "ParkingZone")
                        .WithMany()
                        .HasForeignKey("ParkingZoneId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Office");

                    b.Navigation("OfficeZone");

                    b.Navigation("ParkingZone");
                });

            modelBuilder.Entity("BilleSpace.Infrastructure.Entities.Office", b =>
                {
                    b.Navigation("OfficeZones");

                    b.Navigation("ParkingZones");
                });
#pragma warning restore 612, 618
        }
    }
}
