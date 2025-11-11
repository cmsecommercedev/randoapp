public record EmployeeDto(Guid Id, string FullName, string? Title, string? PhotoUrl, Guid BusinessId);
public record EmployeeCreateDto(string FullName, Guid BusinessId, string? Title, string? PhotoUrl);
