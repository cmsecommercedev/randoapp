public record AppointmentDto(Guid Id, Guid BusinessId, Guid EmployeeId, Guid ServiceId, DateTime StartAt, DateTime EndAt, string? Notes, AppointmentStatus Status, string? CustomerName, string? CustomerPhone);
public record AppointmentCreateDto(Guid BusinessId, Guid EmployeeId, Guid ServiceId, DateTime StartAt, DateTime EndAt, string? Notes, string? CustomerName, string? CustomerPhone);
public record AppointmentStatusUpdateDto(AppointmentStatus Status);
