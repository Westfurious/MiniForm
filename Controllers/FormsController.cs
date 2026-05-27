using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniForm.Dtos.Forms;
using System.Security.Claims;

namespace MiniForm.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FormsController : ControllerBase
{
    private readonly MiniForm.Application.Interfaces.IFormService _formService;

    public FormsController(MiniForm.Application.Interfaces.IFormService formService)
    {
        _formService = formService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<FormResponse>>> GetForms([FromQuery] int page = 1, CancellationToken cancellationToken = default)
    {
        const int pageSize = 10;

        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token." });
        }

        if (page < 1) page = 1;

        var forms = await _formService.GetFormsForUserAsync(userId, page, pageSize, cancellationToken);
        return Ok(forms);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FormResponse>> GetFormById(Guid id, CancellationToken cancellationToken)
    {
        var form = await _formService.GetFormByIdAsync(id, cancellationToken);
        if (form is null)
        {
            return NotFound(new { message = "Form not found." });
        }

        return Ok(form);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<FormResponse>> CreateForm([FromBody] CreateFormRequest request, CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token." });
        }

        var created = await _formService.CreateFormAsync(request, userId, cancellationToken);
        return Ok(created);
    }
}