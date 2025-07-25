using System;

namespace RentReady.API.Dtos
{
    public class PaymentForEditDto
    {
        public int      LeaseId { get; set; }
        public decimal  Amount  { get; set; }
        public DateTime Date    { get; set; }
    }
}