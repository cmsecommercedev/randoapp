using randevuappapi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class AppointmentReview
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    // Appointment ile ilişki
    [Required]
    public Guid AppointmentId { get; set; }
    [ForeignKey(nameof(AppointmentId))]
    public Appointment Appointment { get; set; } = null!;

    // Customer (ApplicationUser) ile ilişki
    [Required]
    public string CustomerId { get; set; } = null!;
    [ForeignKey(nameof(CustomerId))]
    public ApplicationUser Customer { get; set; } = null!;

    // Yorum ve Yıldız
    public string? Comment { get; set; }
    [Range(1, 5)]
    public int? Rating { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}