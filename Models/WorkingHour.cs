using System.ComponentModel.DataAnnotations;

public class WorkingHour
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid BusinessId { get; set; }
    public Business? Business { get; set; }

    [Required]
    public DayOfWeek Day { get; set; }

    [Required]
    public TimeSpan Open { get; set; }

    [Required]
    public TimeSpan Close { get; set; }
}
