namespace RentReady.API.Dtos
{
    public class PropertyDto
    {
        public int    Id         { get; set; }
        public string Address    { get; set; } = null!;
        public string Unit       { get; set; } = null!;
        public decimal RentAmount { get; set; }
    }
}