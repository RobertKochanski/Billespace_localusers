using BilleSpace.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace BilleSpace.Infrastructure
{
    public class BilleSpaceDbContext : DbContext
    {
        public BilleSpaceDbContext(DbContextOptions options) : base(options){}

        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Office> Offices { get; set; }
        public DbSet<OfficeZone> OfficeZones { get; set; }
        public DbSet<ParkingZone> ParkingZones { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Receptionist> Receptionists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Office
            modelBuilder.Entity<Office>()
                .Property(x => x.CityId)
                .IsRequired();

            modelBuilder.Entity<Office>()
                .Property(x => x.Address)
                .IsRequired();

            modelBuilder.Entity<Office>()
                .Property(x => x.PostCode)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}
