namespace RentReady.API.Models
{
    public class Lease
    {
        public int Id { get; set; }                    
        public int PropertyId { get; set; }
        public Property Property { get; set; } = null!;
        public int TenantId   { get; set; }
        public Tenant Tenant  { get; set; }   = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate  { get; set; }

        public ICollection<Payment>? Payments { get; set; }
    }

}