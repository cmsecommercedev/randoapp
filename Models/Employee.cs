using System.ComponentModel.DataAnnotations;

public class Employee
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(150)]
    public string FullName { get; set; } = null!;

    public string? Title { get; set; }
    public string? PhotoUrl { get; set; }

    public Guid BusinessId { get; set; }
    public Business? Business { get; set; }

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
