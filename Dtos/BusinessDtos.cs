public record BusinessListDto(Guid Id, string Name, string? Description, Guid CategoryId);
public record BusinessDetailDto(Guid Id, string Name, string? Description, string? Phone, string? Address, Guid CategoryId, IEnumerable<ServiceDto> Services, IEnumerable<EmployeeDto> Employees);
public record BusinessCreateDto(string Name, string? Description, string? Phone, string? Address, Guid CategoryId, IEnumerable<string>? PhotoUrls);

public class BusinessCreateWithPhotoDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public Guid CategoryId { get; set; }
    public IFormFile? Photo { get; set; }
    public string? PhotoDescription { get; set; }
    public List<IFormFile>? Photos { get; set; } // Çoklu fotoğraf yükleme
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}

public class BusinessCreateResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public Guid CategoryId { get; set; }
    public string? MainPhotoUrl { get; set; }
    public Guid? BusinessPhotoId { get; set; }
    public string? BusinessPhotoUrl { get; set; }
}
