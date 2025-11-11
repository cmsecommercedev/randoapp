using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using randevuappapi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace randevuappapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenManager _tokenManager;


        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration configuration, ITokenManager tokenManager )
        {
            _userManager = userManager;
            _configuration = configuration;
            _tokenManager = tokenManager;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(string phonenumber, string fullName)
        {
            if (string.IsNullOrWhiteSpace(phonenumber))
                return BadRequest("Telefon gerekli.");

            var normalizedPhone = Regex.Replace(phonenumber, @"\D", "");
            string dummyEmail = $"{normalizedPhone}@agx-labs.com";
            string dummyPassword = "DummyPassword123!"; // IdentityUser için zorunlu

            var user = await _userManager.FindByNameAsync(normalizedPhone);
            if (user != null)
            {
                // Kullanıcı varsa OTP yenile
                user.SecurityStamp = "123456"; // Sabit OTP
                await _userManager.UpdateAsync(user);
            }
            else
            {
                user = new ApplicationUser
                {
                    UserName = normalizedPhone,
                    Email = dummyEmail,
                    PhoneNumber = normalizedPhone,
                    FullName = fullName,
                    PhoneNumberConfirmed = false,
                    SecurityStamp = "123456"
                };
                var result = await _userManager.CreateAsync(user, dummyPassword);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { errors });
                }
            }

            // TODO: SMS ile OTP gönder
            Console.WriteLine($"[DEBUG] OTP for {normalizedPhone}: 123456");

            return Ok(new { message = "OTP gönderildi. Lütfen doğrulayın.", userId = user.Id });
        }

        [AllowAnonymous]
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(string phonenumber, string otp)
        {
            if (string.IsNullOrWhiteSpace(phonenumber) || string.IsNullOrWhiteSpace(otp))
                return BadRequest("Telefon ve OTP gerekli.");

            var normalizedPhone = Regex.Replace(phonenumber, @"\D", "");
            var user = await _userManager.FindByNameAsync(normalizedPhone);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            // OTP kontrolü
            if (user.SecurityStamp != otp)
                return BadRequest("Geçersiz OTP.");

            // OTP doğru ise telefon doğrulamasını işaretle
            user.PhoneNumberConfirmed = true;
            user.SecurityStamp = Guid.NewGuid().ToString(); // OTP geçersiz kıl

            // Refresh token oluştur
            user.RefreshToken = _tokenManager.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);

            await _userManager.UpdateAsync(user);

            var accessToken = _tokenManager.GenerateJwtToken(user);

            return Ok(new
            {
                accessToken,
                refreshToken = user.RefreshToken,
                refreshTokenExpiry = user.RefreshTokenExpiryTime
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(string phonenumber)
        {
            if (string.IsNullOrWhiteSpace(phonenumber))
                return BadRequest("Telefon gerekli.");

            var normalizedPhone = Regex.Replace(phonenumber, @"\D", "");
            var user = await _userManager.FindByNameAsync(normalizedPhone);

            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            // OTP üret
            user.SecurityStamp = "123456"; // Şimdilik sabit OTP
            await _userManager.UpdateAsync(user);

            // TODO: SMS ile OTP gönder
            Console.WriteLine($"[DEBUG] OTP for {normalizedPhone}: 123456");

            return Ok(new { message = "OTP gönderildi. Lütfen doğrulayın." });
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(string phonenumber, string refreshToken)
        {
            var normalizedPhone = Regex.Replace(phonenumber, @"\D", "");
            var user = await _userManager.FindByNameAsync(normalizedPhone);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return Unauthorized("Geçersiz veya süresi dolmuş refresh token.");

            var newAccessToken = _tokenManager.GenerateJwtToken(user);

            // Yeni refresh token üret
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
         
    }
}
