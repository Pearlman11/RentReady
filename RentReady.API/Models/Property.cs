namespace RentReady.API.Models
{
    public class Property
    {
        public int Id { get; set; }
        public string Address { get; set; } = null!;
        public string Unit { get; set; } = null!;
        public decimal RentAmount { get; set; }
        public ICollection<Lease>? Leases { get; set; }
    }
}