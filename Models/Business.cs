using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Business
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(200)]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }

    // kategori iliskisi
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }

    // foto url listesi basit tutuyoruz: json string veya ayrı tablo tercih edilebilir
    public string? PhotoUrlsJson { get; set; }

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public ICollection<WorkingHour> WorkingHours { get; set; } = new List<WorkingHour>();
}
