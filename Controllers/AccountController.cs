using Amazon.Runtime.Internal.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using randevuappapi.Models;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly ITokenManager _tokenManager;
    private readonly IMemoryCache _cache;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        ITokenManager tokenManager,
        IMemoryCache cache)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _tokenManager = tokenManager;
        _cache = cache;
    }

    
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(string phonenumber)
    {
        if (string.IsNullOrWhiteSpace(phonenumber))
            return BadRequest("Telefon gerekli.");

        // OTP oluştur
        var otp = new Random().Next(100000, 999999).ToString();

        // OTP'yi cache'e kaydet (5 dakika süreyle)
        _cache.Set(phonenumber, otp, TimeSpan.FromMinutes(5));

        // OTP gönderimi burada yapılabilir (örneğin, SMS servisi ile)
        return Ok(new { message = "OTP gönderildi. Lütfen doğrulayın." });
    }

    // ✅ REFRESH TOKEN
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(string phonenumber, string refreshToken)
    {
        var user = await _userManager.FindByNameAsync(phonenumber);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return Unauthorized("Geçersiz veya süresi dolmuş refresh token.");

        var newAccessToken = await _tokenManager.GenerateJwtTokenAsync(user, _userManager);
        user.RefreshToken = _tokenManager.GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
        await _userManager.UpdateAsync(user);

        return Ok(new
        {
            accessToken = newAccessToken,
            refreshToken = user.RefreshToken,
            refreshTokenExpiry = user.RefreshTokenExpiryTime
        });
    }

    [AllowAnonymous]
    [HttpPost("verify-customer-otp")]
    public async Task<IActionResult> VerifyCustomerOtp(string phonenumber, string    otp)
    {
        return await VerifyOtpWithRoleFromCache(phonenumber, otp, "Customer");
    }

    [AllowAnonymous]
    [HttpPost("verify-business-otp")]
    public async Task<IActionResult> VerifyBusinessOtp(string phonenumber, string otp)
    {
        return await VerifyOtpWithRoleFromCache(phonenumber, otp, "Owner");
    }

    // Ortak OTP doğrulama metodu (cache ile)
    private async Task<IActionResult> VerifyOtpWithRoleFromCache(string phonenumber, string otp, string role)
    {
        if (string.IsNullOrWhiteSpace(phonenumber) || string.IsNullOrWhiteSpace(otp))
            return BadRequest("Telefon ve OTP gerekli.");

        // Cache'den OTP'yi al
        if (!_cache.TryGetValue(phonenumber, out string? cachedOtp) || cachedOtp != otp)
            return BadRequest("Geçersiz veya süresi dolmuş OTP.");

        // OTP doğrulandı, cache'den kaldır
        _cache.Remove(phonenumber);

        // Kullanıcıyı kontrol et
        var user = await _userManager.FindByNameAsync(phonenumber);
        if (user == null)
        {
            // Kullanıcı yoksa oluştur ve rol ata
            string dummyEmail = $"{phonenumber}@agx-labs.com";
            string dummyPassword = "DummyPassword123!";

            user = new ApplicationUser
            {
                UserName = phonenumber,
                Email = dummyEmail,
                PhoneNumber = phonenumber,
                PhoneNumberConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, dummyPassword);
            if (!createResult.Succeeded)
                return BadRequest(createResult.Errors.Select(e => e.Description));

            await _userManager.AddToRoleAsync(user, role);
        }

        // Refresh token oluştur ve kaydet
        user.RefreshToken = _tokenManager.GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
        await _userManager.UpdateAsync(user);

        // Kullanıcı claim’leri ekle
        var roles = await _userManager.GetRolesAsync(user);
        var userRole = roles.FirstOrDefault() ?? "";
        var defaultClaims = new List<Claim>
        {
            new Claim("FullName", user.FullName ?? ""),
            new Claim("Email", user.Email ?? ""),
            new Claim("UserRole", userRole),
            new Claim("ProfilePhotoUrl", user.ProfilePhotoUrl ?? "")
        };

        var existingClaims = await _userManager.GetClaimsAsync(user);
        foreach (var claim in defaultClaims)
        {
            if (!existingClaims.Any(c => c.Type == claim.Type))
                await _userManager.AddClaimAsync(user, claim);
        }

        // JWT oluştur
        var accessToken = await _tokenManager.GenerateJwtTokenAsync(user, _userManager);

        return Ok(new
        {
            accessToken,
            refreshToken = user.RefreshToken,
            refreshTokenExpiry = user.RefreshTokenExpiryTime
        });
    }
}
