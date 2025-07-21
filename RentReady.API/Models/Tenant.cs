using System.Collections.Generic;
namespace RentReady.API.Models
{
    public class Tenant
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public ICollection<Lease>? Leases { get; set; }


    }
}
  