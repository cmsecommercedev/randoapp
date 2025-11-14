using System.ComponentModel.DataAnnotations;

public enum AppointmentStatus
{
    Pending,
    Approved,
    Cancelled,
    Completed
}

public class Appointment
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid BusinessId { get; set; }
    public Business? Business { get; set; }

    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public Guid ServiceId { get; set; }
    public Service? Service { get; set; }

    [Required]
    public DateTime StartAt { get; set; }

    [Required]
    public DateTime EndAt { get; set; }

    [Required]
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    public string? Notes { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // müşteri bilgilerini basit tutuyoruz (daha sonra AspNetUsers ile ilişkilendirilebilir)
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
}
