using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniForm.Data;
using MiniForm.Models;

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

    [HttpGet]
    public async Task<ActionResult<List<Form>>> GetForms()
    {
        return await _formService.GetFormsAsync();
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Form>> CreateForm(Form form, CancellationToken cancellationToken)
    {
        var created = await _formService.CreateFormAsync(form, cancellationToken);
        return Ok(created);
    }
}