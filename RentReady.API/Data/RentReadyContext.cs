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
            



    }

    
}