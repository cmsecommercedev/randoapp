using Microsoft.AspNetCore.Identity;

namespace randevuappapi.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Ek kullanıcı özellikleri eklemek isterseniz buraya ekleyebilirsiniz.
        public string? FullName { get; set; }
        // Mobil için refresh token
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
