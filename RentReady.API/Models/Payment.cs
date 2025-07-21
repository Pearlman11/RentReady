using System;

namespace RentReady.API.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int LeaseId { get; set; }
        public Lease Lease { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        
    }
}