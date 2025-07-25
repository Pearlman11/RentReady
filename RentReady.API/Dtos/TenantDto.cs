namespace RentReady.API.Dtos
{
    public class TenantDto
    {
        public int    Id          { get; set; }
        public string Name        { get; set; } = null!;
        public string Email       { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        
        // Expanded leases inline
        public ICollection<LeaseSummaryDto>? Leases { get; set; }
    }
}