public interface IAppointmentManager
{
    Task<IEnumerable<AppointmentDto>> GetByBusinessDateAsync(Guid businessId, DateTime date);
    Task<AppointmentDto> CreateAsync(AppointmentCreateDto dto);
    Task<bool> UpdateStatusAsync(Guid appointmentId, AppointmentStatus status);
}
