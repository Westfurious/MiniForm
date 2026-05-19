using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniForm.Data;
using MiniForm.Models;
using MiniForm.Data;

namespace MiniForm.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FormsController : ControllerBase
{
    private readonly AppDbContext _context;

    public FormsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Form>>> GetForms()
    {
        return await _context.Forms.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Form>> CreateForm(Form form)
    {
        _context.Forms.Add(form);

        await _context.SaveChangesAsync();

        return Ok(form);
    }
}