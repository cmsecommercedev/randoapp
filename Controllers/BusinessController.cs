using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using randevuappapi.Data;
using randevuappapi.Models;
using System.Security.Claims;
using System.IO;
[ApiController]
[Route("api/[controller]")]
public class BusinessController : ControllerBase
{
    private readonly IBusinessManager _mgr;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly CloudflareR2Manager _r2Manager;

    public BusinessController(IBusinessManager mgr, UserManager<ApplicationUser> userManager, ApplicationDbContext context, CloudflareR2Manager r2Manager)
    {
        _mgr = mgr;
        _userManager = userManager;
        _context = context;
        _r2Manager = r2Manager;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _mgr.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _mgr.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    } 

    [HttpPost("create-with-photo")]
    [Authorize(Roles = "Owner,Admin")]
    [ProducesResponseType(typeof(BusinessCreateResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BusinessCreateResponseDto>> CreateBusinessWithPhoto([FromForm] BusinessCreateWithPhotoDto dto)
    {
        // JWT'den userId'yi al
        var userId = User.FindFirstValue("UserId");
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User not authenticated");
        }

        // Kullanıcıyı kontrol et
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return BadRequest("User not found");
        }

        string? mainPhotoUrl = null;
        Guid? businessPhotoId = null;
        string? businessPhotoUrl = null;
        var businessPhotoIds = new List<Guid>();
        var businessPhotoUrls = new List<string>();

        // Çoklu fotoğraf yükleme
        if (dto.Photos != null && dto.Photos.Count > 0)
        {
            foreach (var photo in dto.Photos)
            {
                if (photo != null && photo.Length > 0)
                {
                    try
                    {
                        var fileExtension = Path.GetExtension(photo.FileName);
                        var fileName = $"business/{Guid.NewGuid()}{fileExtension}";
                        var contentType = photo.ContentType;
                        using (var stream = photo.OpenReadStream())
                        {
                            await _r2Manager.UploadFileAsync(fileName, stream, contentType);
                        }
                        var url = _r2Manager.GetFileUrl(fileName);
                        businessPhotoUrls.Add(url);
                        // İlk fotoğraf ana fotoğraf olarak atanır
                        if (mainPhotoUrl == null)
                            mainPhotoUrl = url;
                    }
                    catch (Exception ex)
                    {
                        return BadRequest($"Photo upload failed: {ex.Message}");
                    }
                }
            }
        }
        else if (dto.Photo != null && dto.Photo.Length > 0) // Geriye dönük uyumluluk için
        {
            try
            {
                var fileExtension = Path.GetExtension(dto.Photo.FileName);
                var fileName = $"business/{Guid.NewGuid()}{fileExtension}";
                var contentType = dto.Photo.ContentType;
                using (var stream = dto.Photo.OpenReadStream())
                {
                    await _r2Manager.UploadFileAsync(fileName, stream, contentType);
                }
                mainPhotoUrl = _r2Manager.GetFileUrl(fileName);
                businessPhotoUrls.Add(mainPhotoUrl);
            }
            catch (Exception ex)
            {
                return BadRequest($"Photo upload failed: {ex.Message}");
            }
        }

        // Business oluştur
        var business = new Business
        {
            Name = dto.Name,
            Description = dto.Description,
            Phone = dto.Phone,
            Address = dto.Address,
            CategoryId = dto.CategoryId,
            OwnerId = userId,
            MainPhotoUrl = mainPhotoUrl,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };

        _context.Businesses.Add(business);
        await _context.SaveChangesAsync();

        // BusinessPhoto kayıtları oluştur
        if (businessPhotoUrls.Count > 0)
        {
            foreach (var url in businessPhotoUrls)
            {
                var businessPhoto = new BusinessPhoto
                {
                    BusinessId = business.Id,
                    Url = url,
                    Description = dto.PhotoDescription // Her foto için aynı açıklama atanıyor, istenirse dto'da liste yapılabilir
                };
                _context.BusinessPhotos.Add(businessPhoto);
                await _context.SaveChangesAsync();
                businessPhotoIds.Add(businessPhoto.Id);
            }
            businessPhotoId = businessPhotoIds.FirstOrDefault();
            businessPhotoUrl = businessPhotoUrls.FirstOrDefault();
        }

        var response = new BusinessCreateResponseDto
        {
            Id = business.Id,
            Name = business.Name,
            Description = business.Description,
            Phone = business.Phone,
            Address = business.Address,
            CategoryId = business.CategoryId,
            MainPhotoUrl = business.MainPhotoUrl,
            BusinessPhotoId = businessPhotoId,
            BusinessPhotoUrl = businessPhotoUrl
        };

        return CreatedAtAction(nameof(Get), new { id = business.Id }, response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBusiness(string name, string description, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return BadRequest("User not found");
        }

        var business = new Business
        {
            Name = name,
            Description = description,
            OwnerId = user.Id
        };

        _context.Businesses.Add(business);
        await _context.SaveChangesAsync();

        return Ok("Business created successfully");
    }
}
