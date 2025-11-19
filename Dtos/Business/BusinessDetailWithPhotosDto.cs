namespace randevuappapi.Dtos.Business
{
    public class BusinessDetailWithPhotosDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? CategoryIcon { get; set; }
        public string? MainPhotoUrl { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public List<BusinessPhotoDto> Photos { get; set; } = new();
    }

    public class BusinessPhotoDto
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = null!;
        public string? Description { get; set; }
    }
}
