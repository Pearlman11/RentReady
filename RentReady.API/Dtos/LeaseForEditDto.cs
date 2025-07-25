namespace RentReady.API.Dtos
{
    public class LeaseForEditDto
    {
        public int      PropertyId { get; set; }
        public int      TenantId   { get; set; }
        public DateTime StartDate  { get; set; }
        public DateTime? EndDate   { get; set; }
    }
}