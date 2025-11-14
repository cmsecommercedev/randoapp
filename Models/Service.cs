using System.ComponentModel.DataAnnotations;

public class Service
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(150)]
    public string Name { get; set; } = null!;

    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }

    public Guid BusinessId { get; set; }
    public Business? Business { get; set; }

    public Guid? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
