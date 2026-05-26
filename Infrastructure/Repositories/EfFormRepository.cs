using Microsoft.EntityFrameworkCore;
using MiniForm.Application.Interfaces;
using MiniForm.Data;
using MiniForm.Models;

namespace MiniForm.Infrastructure.Repositories;

public class EfFormRepository : IFormRepository
{
    private readonly AppDbContext _context;

    public EfFormRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Form>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Forms.ToListAsync(cancellationToken);
    }

    public async Task<Form> AddAsync(Form form, CancellationToken cancellationToken = default)
    {
        _context.Forms.Add(form);
        return form;
    }
}
