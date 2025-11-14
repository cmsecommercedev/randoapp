using Microsoft.AspNetCore.Identity;
using randevuappapi.Models;

public interface ITokenManager
{
    Task<string> GenerateJwtTokenAsync(ApplicationUser user, UserManager<ApplicationUser> userManager);
    string GenerateRefreshToken();
}
