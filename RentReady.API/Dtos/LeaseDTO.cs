using System;

namespace RentReady.API.Dtos
{
    public class LeaseDto
    {
        public int    Id         { get; set; }
        public int    PropertyId { get; set; }
        public int    TenantId   { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate  { get; set; }

        // Expanded details
        public PropertyDto Property { get; set; } = null!;
        public TenantDto   Tenant   { get; set; } = null!;
    }
}
