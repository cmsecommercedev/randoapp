using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using randevuappapi.Data;
using randevuappapi.Dtos.Appointment;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize] // JWT doğrulaması için
public class AppointmentReviewController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AppointmentReviewController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddReview([FromBody] AppointmentReviewDto dto)
    {
        // JWT'den gelen kullanıcı kimliği
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (customerId == null)
        {
            return Unauthorized("User not authenticated");
        }

        // Appointment kontrolü
        var appointment = await _context.Appointments.FindAsync(dto.AppointmentId);
        if (appointment == null)
        {
            return NotFound("Appointment not found");
        }

        // Kullanıcının bu randevunun müşterisi olup olmadığını kontrol et
        if (appointment.CustomerId.ToString() != customerId)
        {
            return Forbid("You are not authorized to review this appointment");
        }

        // Yeni yorum oluştur
        var review = new AppointmentReview
        {
            AppointmentId = dto.AppointmentId,
            CustomerId = customerId,
            Comment = dto.Comment,
            Rating = dto.Rating
        };

        _context.AppointmentReviews.Add(review);
        await _context.SaveChangesAsync();

        return Ok("Review added successfully");
    }
}

