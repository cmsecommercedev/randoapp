public record BusinessListDto(Guid Id, string Name, string? Description, Guid CategoryId);
public record BusinessDetailDto(Guid Id, string Name, string? Description, string? Phone, string? Address, Guid CategoryId, IEnumerable<ServiceDto> Services, IEnumerable<EmployeeDto> Employees);
public record BusinessCreateDto(string Name, string? Description, string? Phone, string? Address, Guid CategoryId, IEnumerable<string>? PhotoUrls);
