using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace randevuappapi.Models
{
    public class UserSettings
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        // Profil bilgileri
        public string? ProfilePhotoUrl { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }

        // Konum bilgileri
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Diğer ayarlar (örnek olarak)
        public bool NotificationsEnabled { get; set; } = true;
    }
}
