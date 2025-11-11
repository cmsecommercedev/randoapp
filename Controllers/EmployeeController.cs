using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeManager _mgr;
    public EmployeeController(IEmployeeManager mgr) { _mgr = mgr; }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id) => OkOrNotFound(await _mgr.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Roles = "BusinessAdmin,Admin")]
    public async Task<IActionResult> Create([FromBody] EmployeeCreateDto dto)
    {
        var created = await _mgr.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpGet("{id:guid}/availability")]
    public async Task<IActionResult> Availability(Guid id, [FromQuery] DateTime date)
    {
        var slots = await _mgr.GetAvailabilityAsync(id, date);
        return Ok(slots);
    }

    private IActionResult OkOrNotFound(object? obj) => obj == null ? NotFound() : Ok(obj);
}
