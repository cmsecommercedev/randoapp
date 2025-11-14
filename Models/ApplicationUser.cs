using Microsoft.AspNetCore.Identity;

namespace randevuappapi.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? OtpCode { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        // Profil bilgileri
        public string? ProfilePhotoUrl { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public virtual UserSettings? UserSettings { get; set; }


    }
}
