using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using randevuappapi.Data;
using randevuappapi.Models;
[ApiController]
[Route("api/[controller]")]
public class BusinessController : ControllerBase
{
    private readonly IBusinessManager _mgr;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public BusinessController(IBusinessManager mgr, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _mgr = mgr;
        _userManager = userManager;
        _context = context;
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

    [HttpPost]
    [Authorize(Roles = "Owner,Admin")]
    public async Task<IActionResult> Create([FromBody] BusinessCreateDto dto)
    {
        var created = await _mgr.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
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
