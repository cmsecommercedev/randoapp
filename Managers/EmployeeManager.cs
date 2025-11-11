using Microsoft.EntityFrameworkCore;
using randevuappapi.Data;

public class EmployeeManager : IEmployeeManager
{
    private readonly ApplicationDbContext _db;

    public EmployeeManager(ApplicationDbContext db) { _db = db; }

    public async Task<EmployeeDto?> GetByIdAsync(Guid id)
    {
        var e = await _db.Employees.FindAsync(id);
        if (e == null) return null;
        return new EmployeeDto(e.Id, e.FullName, e.Title, e.PhotoUrl, e.BusinessId);
    }

    public async Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto)
    {
        var e = new Employee { FullName = dto.FullName, BusinessId = dto.BusinessId, Title = dto.Title, PhotoUrl = dto.PhotoUrl };
        _db.Employees.Add(e);
        await _db.SaveChangesAsync();
        return new EmployeeDto(e.Id, e.FullName, e.Title, e.PhotoUrl, e.BusinessId);
    }

    // Basit availability hesaplama: working hours ve mevcut randevuları kullanır.
    public async Task<IEnumerable<TimeSpan>> GetAvailabilityAsync(Guid employeeId, DateTime date)
    {
        var emp = await _db.Employees.Include(x => x.Business).ThenInclude(b => b.WorkingHours).FirstOrDefaultAsync(x => x.Id == employeeId);
        if (emp == null) return Enumerable.Empty<TimeSpan>();

        var day = date.DayOfWeek;
        var wh = emp.Business!.WorkingHours.FirstOrDefault(w => w.Day == day);
        if (wh == null) return Enumerable.Empty<TimeSpan>();

        // 30 dk slot örneği. Daha sofistike olmasını istersen service.duration’u ekle.
        var slot = TimeSpan.FromMinutes(30);
        var start = date.Date + wh.Open;
        var end = date.Date + wh.Close;

        var appointments = await _db.Appointments
            .Where(a => a.EmployeeId == employeeId && a.StartAt.Date == date.Date && a.Status != AppointmentStatus.Cancelled)
            .ToListAsync();

        var freeSlots = new List<TimeSpan>();
        for (var t = start; t + slot <= end; t += slot)
        {
            var slotStart = t;
            var slotEnd = t + slot;
            var conflict = appointments.Any(a => a.StartAt < slotEnd && a.EndAt > slotStart);
            if (!conflict) freeSlots.Add(slotStart.TimeOfDay);
        }
        return freeSlots;
    }
}
