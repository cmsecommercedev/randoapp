using Microsoft.EntityFrameworkCore;
using randevuappapi.Data;

public class AppointmentManager : IAppointmentManager
{
    private readonly ApplicationDbContext _db;
    public AppointmentManager(ApplicationDbContext db) { _db = db; }

    public async Task<IEnumerable<AppointmentDto>> GetByBusinessDateAsync(Guid businessId, DateTime date)
    {
        var list = await _db.Appointments
            .Where(a => a.BusinessId == businessId && a.StartAt.Date == date.Date)
            .ToListAsync();

        return list.Select(a => new AppointmentDto(a.Id, a.BusinessId, a.EmployeeId, a.ServiceId, a.StartAt, a.EndAt, a.Notes, a.Status, a.CustomerName, a.CustomerPhone));
    }

    public async Task<AppointmentDto> CreateAsync(AppointmentCreateDto dto)
    {
        // Basit validasyonlar:
        if (dto.EndAt <= dto.StartAt) throw new ArgumentException("EndAt must be after StartAt");

        // Çakışma kontrolü: aynı çalışan aralığında randevu var mı?
        var conflict = await _db.Appointments.AnyAsync(a =>
            a.EmployeeId == dto.EmployeeId &&
            a.Status != AppointmentStatus.Cancelled &&
            a.StartAt < dto.EndAt &&
            a.EndAt > dto.StartAt);

        if (conflict) throw new InvalidOperationException("Time slot conflicts with existing appointment");

        var entity = new Appointment
        {
            BusinessId = dto.BusinessId,
            EmployeeId = dto.EmployeeId,
            ServiceId = dto.ServiceId,
            StartAt = dto.StartAt,
            EndAt = dto.EndAt,
            Notes = dto.Notes,
            CustomerName = dto.CustomerName,
            CustomerPhone = dto.CustomerPhone,
            Status = AppointmentStatus.Pending
        };

        _db.Appointments.Add(entity);
        await _db.SaveChangesAsync();

        return new AppointmentDto(entity.Id, entity.BusinessId, entity.EmployeeId, entity.ServiceId, entity.StartAt, entity.EndAt, entity.Notes, entity.Status, entity.CustomerName, entity.CustomerPhone);
    }

    public async Task<bool> UpdateStatusAsync(Guid appointmentId, AppointmentStatus status)
    {
        var a = await _db.Appointments.FindAsync(appointmentId);
        if (a == null) return false;
        a.Status = status;
        await _db.SaveChangesAsync();
        return true;
    }
}
