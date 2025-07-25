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
                new Property { Id = 2, Address = "456 Oak Ave", Unit = "Unit B", RentAmount = 1500m }
            );
            
            // Seed Tenants
            modelBuilder.Entity<Tenant>().HasData(
                new Tenant { Id = 1, Name = "Alice Johnson",  Email = "alice@example.com",   PhoneNumber = "555-1234" },
                new Tenant { Id = 2, Name = "Bob Williams",   Email = "bob@example.com",     PhoneNumber = "555-5678" }
            );

            // Seed Leases (must reference existing PropertyId and TenantId)
            modelBuilder.Entity<Lease>().HasData(
                new Lease { Id = 1, PropertyId = 1, TenantId = 1, StartDate = new DateTime(2025,1,1), EndDate = null },
                new Lease { Id = 2, PropertyId = 2, TenantId = 2, StartDate = new DateTime(2025,3,1), EndDate = null }
            );

            // Seed Payments (reference existing LeaseId)
            modelBuilder.Entity<Payment>().HasData(
                new Payment { Id = 1, LeaseId = 1, Amount = 1200m, Date = new DateTime(2025,1,5) },
                new Payment { Id = 2, LeaseId = 2, Amount = 1500m, Date = new DateTime(2025,3,5) }
            );
        }
            



    }

    
}