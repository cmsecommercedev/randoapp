public record ServiceDto(Guid Id, string Name, int DurationMinutes, decimal Price);
public record ServiceCreateDto(string Name, int DurationMinutes, decimal Price, Guid BusinessId);
