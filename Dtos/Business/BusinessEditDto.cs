namespace randevuappapi.Dtos.Business
{
    public class BusinessEditDto
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public Guid CategoryId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
