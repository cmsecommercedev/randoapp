using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/[controller]")]
public class BusinessController : ControllerBase
{
    private readonly IBusinessManager _mgr;
    public BusinessController(IBusinessManager mgr) { _mgr = mgr; }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _mgr.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var item = await _mgr.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "BusinessAdmin,Admin")]
    public async Task<IActionResult> Create([FromBody] BusinessCreateDto dto)
    {
        var created = await _mgr.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }
}
