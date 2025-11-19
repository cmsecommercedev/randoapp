using System.ComponentModel.DataAnnotations;

public class Category
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;
    public string Icon { get; set; } = null!;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
