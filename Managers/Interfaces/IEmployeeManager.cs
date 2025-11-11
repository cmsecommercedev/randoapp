public interface IEmployeeManager
{
    Task<EmployeeDto?> GetByIdAsync(Guid id);
    Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto);
    Task<IEnumerable<TimeSpan>> GetAvailabilityAsync(Guid employeeId, DateTime date);
}
