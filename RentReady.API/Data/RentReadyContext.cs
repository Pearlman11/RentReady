using Microsoft.EntityFrameworkCore;
using RentReady.API.Models;
namespace RentReady.API.Data
{
    public class RentReadyContext : DbContext
    {
        public RentReadyContext(DbContextOptions<RentReadyContext> options) : base(options)
        {

        }

        public DbSet<Property> Properties => Set<Property>();
        public DbSet<Lease> Leases => Set<Lease>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Tenant> Tenants => Set<Tenant>();

          protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed sample properties
            modelBuilder.Entity<Property>().HasData(
                new Property { Id = 1, Address = "123 Main St", Unit = "Apt 101", RentAmount = 1200m },
                new Property { Id = 2, Address = "456 Oak Ave", Unit = "Unit B",  RentAmount = 1500m }
            );
        }
            



    }

    
}