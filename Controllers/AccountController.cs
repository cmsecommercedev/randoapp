using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration,
        ITokenManager tokenManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _tokenManager = tokenManager;
    }

    // ✅ REGISTER
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(string phonenumber, string fullName)
    {
        if (string.IsNullOrWhiteSpace(phonenumber))
            return BadRequest("Telefon gerekli.");

        string dummyEmail = $"{phonenumber}@agx-labs.com";
        string dummyPassword = "DummyPassword123!";

        var user = await _userManager.FindByNameAsync(phonenumber);
       //var otp = new Random().Next(100000, 999999).ToString();

        if (user != null)
        {
            user.OtpCode = "123456";
            await _userManager.UpdateAsync(user);
        }
        else
        {
            user = new ApplicationUser
            {
                UserName = phonenumber,
                Email = dummyEmail,
                PhoneNumber = phonenumber,
                FullName = fullName,
                PhoneNumberConfirmed = false,
                OtpCode = "123456"
            };

            var result = await _userManager.CreateAsync(user, dummyPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));
        }

        return Ok(new { message = "OTP gönderildi. Lütfen doğrulayın.", userId = user.Id });
    }

    // ✅ REGISTER OWNER
    [AllowAnonymous]
    [HttpPost("register-owner")]
    public async Task<IActionResult> RegisterOwner(string phonenumber, string fullName)
    {
        return await RegisterWithRole(phonenumber, fullName, "Owner");
    }

    // ✅ REGISTER EMPLOYEE
    [AllowAnonymous]
    [HttpPost("register-employee")]
    public async Task<IActionResult> RegisterEmployee(string phonenumber, string fullName)
    {
        return await RegisterWithRole(phonenumber, fullName, "Employee");
    }

    // ✅ REGISTER CUSTOMER
    [AllowAnonymous]
    [HttpPost("register-customer")]
    public async Task<IActionResult> RegisterCustomer(string phonenumber, string fullName)
    {
        return await RegisterWithRole(phonenumber, fullName, "Customer");
    }

    // Ortak register metodu (rol ile)
    private async Task<IActionResult> RegisterWithRole(string phonenumber, string fullName, string role)
    {
        if (string.IsNullOrWhiteSpace(phonenumber))
            return BadRequest("Telefon gerekli.");

        string dummyEmail = $"{phonenumber}@agx-labs.com";
        string dummyPassword = "DummyPassword123!";

        var user = await _userManager.FindByNameAsync(phonenumber);
        if (user != null)
        {
            user.OtpCode = "123456";
            await _userManager.UpdateAsync(user);
        }
        else
        {
            user = new ApplicationUser
            {
                UserName = phonenumber,
                Email = dummyEmail,
                PhoneNumber = phonenumber,
                FullName = fullName,
                PhoneNumberConfirmed = false,
                OtpCode = "123456"
            };
            var result = await _userManager.CreateAsync(user, dummyPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));
            // Rol ata
            await _userManager.AddToRoleAsync(user, role);
        }
        return Ok(new { message = "OTP gönderildi. Lütfen doğrulayın.", userId = user.Id });
    }

    // ✅ LOGIN (OTP gönderimi)
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(string phonenumber)
    {
        if (string.IsNullOrWhiteSpace(phonenumber))
            return BadRequest("Telefon gerekli.");

        var user = await _userManager.FindByNameAsync(phonenumber);

        if (user == null)
            return NotFound("Kullanıcı bulunamadı.");

        //var otp = new Random().Next(100000, 999999).ToString();

        user.OtpCode = "123456";
        await _userManager.UpdateAsync(user);

        // OTP gönderimi burada (SMS servisine entegre edilebilir)
        return Ok(new { message = "OTP gönderildi. Lütfen doğrulayın." });
    }

    // ✅ OTP DOĞRULAMA + TOKEN ÜRETİMİ
    [AllowAnonymous]
    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp(string phonenumber, string otp)
    {
        if (string.IsNullOrWhiteSpace(phonenumber) || string.IsNullOrWhiteSpace(otp))
            return BadRequest("Telefon ve OTP gerekli.");

        var user = await _userManager.FindByNameAsync(phonenumber);
        if (user == null)
            return NotFound("Kullanıcı bulunamadı.");

        if (user.OtpCode != otp)
            return BadRequest("Geçersiz OTP.");

        user.PhoneNumberConfirmed = true;
        user.OtpCode = null;
        user.RefreshToken = _tokenManager.GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
        await _userManager.UpdateAsync(user);

        // Kullanıcı claim’leri ekle (eğer yoksa)
        var defaultClaims = new List<Claim>
        {
            new Claim("FullName", user.FullName ?? ""),
            new Claim("Email", user.Email ?? ""),
            new Claim("ProfilePhotoUrl", user.ProfilePhotoUrl ?? "")
        };

        var existingClaims = await _userManager.GetClaimsAsync(user);
        foreach (var claim in defaultClaims)
        {
            if (!existingClaims.Any(c => c.Type == claim.Type))
                await _userManager.AddClaimAsync(user, claim);
        }

        var accessToken = await _tokenManager.GenerateJwtTokenAsync(user, _userManager);

        return Ok(new
        {
            accessToken,
            refreshToken = user.RefreshToken,
            refreshTokenExpiry = user.RefreshTokenExpiryTime
        });
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
    public async Task<IActionResult> VerifyCustomerOtp(string phonenumber, string otp)
    {
        return await VerifyOtpWithRole(phonenumber, otp, "Customer");
    }

    [AllowAnonymous]
    [HttpPost("verify-business-otp")]
    public async Task<IActionResult> VerifyBusinessOtp(string phonenumber, string otp)
    {
        return await VerifyOtpWithRole(phonenumber, otp, "Owner");
    }

    // Ortak OTP doğrulama metodu (rol ile)
    private async Task<IActionResult> VerifyOtpWithRole(string phonenumber, string otp, string role)
    {
        if (string.IsNullOrWhiteSpace(phonenumber) || string.IsNullOrWhiteSpace(otp))
            return BadRequest("Telefon ve OTP gerekli.");

        var user = await _userManager.FindByNameAsync(phonenumber);

        if (user == null)
        {
            // Kullanıcı yoksa yeni bir kullanıcı oluştur ve ilgili rolü ata
            string dummyEmail = $"{phonenumber}@agx-labs.com";
            string dummyPassword = "DummyPassword123!";

            user = new ApplicationUser
            {
                UserName = phonenumber,
                Email = dummyEmail,
                PhoneNumber = phonenumber,
                PhoneNumberConfirmed = true,
                OtpCode = null // OTP doğrulandıktan sonra sıfırlanır
            };

            var createResult = await _userManager.CreateAsync(user, dummyPassword);
            if (!createResult.Succeeded)
                return BadRequest(createResult.Errors.Select(e => e.Description));

            // Rol ata
            await _userManager.AddToRoleAsync(user, role);
        }
        else
        {
            // Kullanıcı varsa OTP doğrula
            if (user.OtpCode != otp)
                return BadRequest("Geçersiz OTP.");

            user.PhoneNumberConfirmed = true;
            user.OtpCode = null; // OTP doğrulandıktan sonra sıfırlanır
        }

        // Refresh token oluştur ve kaydet
        user.RefreshToken = _tokenManager.GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
        await _userManager.UpdateAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var userRole = roles.FirstOrDefault() ?? "";
        // Kullanıcı claim’leri ekle (eğer yoksa)
        var defaultClaims = new List<Claim>
         {
             new Claim("FullName", user.FullName ?? ""),
             new Claim("Email", user.Email ?? ""),
             new Claim("UserRole", userRole ?? ""),
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
