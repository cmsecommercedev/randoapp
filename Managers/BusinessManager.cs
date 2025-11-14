using System;
using Microsoft.EntityFrameworkCore;
using randevuappapi.Data;

public class BusinessManager : IBusinessManager
{
    private readonly ApplicationDbContext _db;
    public BusinessManager(ApplicationDbContext db) { _db = db; }

    public async Task<IEnumerable<BusinessListDto>> GetAllAsync()
    {
        return await _db.Businesses
            .Select(b => new BusinessListDto(b.Id, b.Name, b.Description, b.CategoryId))
            .ToListAsync();
    }

    public async Task<BusinessDetailDto?> GetByIdAsync(Guid id)
    {
        var b = await _db.Businesses
            .Include(x => x.Services)
            .Include(x => x.Employees)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (b is null) return null;

        var services = b.Services.Select(s => new ServiceDto(s.Id, s.Name, s.DurationMinutes, s.Price));
        var employees = b.Employees.Select(e => new EmployeeDto(e.Id, e.FullName, e.Title, e.PhotoUrl, e.BusinessId));
        return new BusinessDetailDto(b.Id, b.Name, b.Description, b.Phone, b.Address, b.CategoryId, services, employees);
    }

    public async Task<BusinessListDto> CreateAsync(BusinessCreateDto dto)
    {
        var entity = new Business
        {
            Name = dto.Name,
            Description = dto.Description,
            Phone = dto.Phone,
            Address = dto.Address,
            CategoryId = dto.CategoryId
        };

        _db.Businesses.Add(entity);
        await _db.SaveChangesAsync();

        return new BusinessListDto(entity.Id, entity.Name, entity.Description, entity.CategoryId);
    }
}
