using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class BusinessPhoto
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    [Required, MaxLength(500)]
    public string Url { get; set; } = null!;

    public string? Description { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
