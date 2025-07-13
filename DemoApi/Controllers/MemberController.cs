using Microsoft.AspNetCore.Mvc;
using Demo.Data;

[ApiController]
[Route("api/[controller]")]
public class MemberController : ControllerBase
{
    private readonly AppDbContext _context;

    public MemberController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_context.Members.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(string id)
    {
        var member = _context.Members.Find(id);
        if (member == null) return NotFound();
        return Ok(member);
    }
}