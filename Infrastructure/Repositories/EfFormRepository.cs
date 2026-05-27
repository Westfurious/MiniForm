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

    public async Task<List<Form>> GetByUserAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        var skip = (page - 1) * pageSize;
        return await _context.Forms
            .Where(f => f.CreatedByUserId == userId)
            .Include(f => f.Questions)
            .OrderByDescending(f => f.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Form?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Forms
            .Include(form => form.Questions)
            .SingleOrDefaultAsync(form => form.Id == id, cancellationToken);
    }

    public async Task<Form> AddAsync(Form form, CancellationToken cancellationToken = default)
    {
        _context.Forms.Add(form);
        return form;
    }
}
