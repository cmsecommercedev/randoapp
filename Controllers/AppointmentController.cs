using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentManager _mgr;
    public AppointmentController(IAppointmentManager mgr) { _mgr = mgr; }

    [HttpGet]
    public async Task<IActionResult> GetByBusinessDate([FromQuery] Guid businessId, [FromQuery] DateTime date)
    {
        return Ok(await _mgr.GetByBusinessDateAsync(businessId, date));
    }

    [HttpPost]
    [Authorize] // müşteri veya yetkili oluşturabilir
    public async Task<IActionResult> Create([FromBody] AppointmentCreateDto dto)
    {
        try
        {
            var created = await _mgr.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByBusinessDate), new { businessId = created.BusinessId, date = created.StartAt.Date }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "BusinessAdmin,Admin")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] AppointmentStatusUpdateDto dto)
    {
        var ok = await _mgr.UpdateStatusAsync(id, dto.Status);
        if (!ok) return NotFound();
        return NoContent();
    }
}
