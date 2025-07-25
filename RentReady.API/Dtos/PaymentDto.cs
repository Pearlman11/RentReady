using System;

namespace RentReady.API.Dtos
{
    public class PaymentDto
    {
        public int    Id      { get; set; }
        public int    LeaseId { get; set; }
        public decimal Amount  { get; set; }
        public DateTime Date   { get; set; }

        // Expanded lease details
        public LeaseDto Lease { get; set; } = null!;
    }
}