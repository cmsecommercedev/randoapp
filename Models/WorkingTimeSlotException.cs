using System;
using System.ComponentModel.DataAnnotations;

public class WorkingTimeSlotException
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    public Guid? EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    [Required]
    public DateTime Date { get; set; } // İstisna günü

    [Required]
    public TimeSpan Start { get; set; }

    [Required]
    public TimeSpan End { get; set; }

    public string? Reason { get; set; } // ör: bakım, izin, vs.

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
