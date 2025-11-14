using randevuappapi.Models;
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

    // Ana dükkan fotoğrafı
    public string? MainPhotoUrl { get; set; }

    // Konum bilgisi
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    // Fotoğraf tablosu ile ilişki
    public ICollection<BusinessPhoto> Photos { get; set; } = new List<BusinessPhoto>();

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<WorkingHour> WorkingHours { get; set; } = new List<WorkingHour>();

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Kullanıcı ile ilişki
    [Required]
    public string OwnerId { get; set; } = null!; // Foreign key
    [ForeignKey(nameof(OwnerId))]
    public ApplicationUser Owner { get; set; } = null!;
}
