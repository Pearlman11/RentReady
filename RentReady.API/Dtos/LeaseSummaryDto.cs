namespace RentReady.API.Dtos
{
    public class LeaseSummaryDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    
}