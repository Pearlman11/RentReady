namespace RentReady.API.Dtos
{
    public class PropertyForEditDto
    {
        public string Address     { get; set; } = null!;
        public string Unit        { get; set; } = null!;
        public decimal RentAmount { get; set; }
    }
}