using Microsoft.EntityFrameworkCore;
using MiniForm.Application.Interfaces;
using MiniForm.Data;
using MiniForm.Models;

namespace MiniForm.Infrastructure.Repositories;

public class EfAnalyticsRepository : IAnalyticsRepository
{
    private readonly AppDbContext _context;

    public EfAnalyticsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Submission>> GetSubmissionsWithAnswersAsync(Guid formId, CancellationToken cancellationToken = default)
    {
        return await _context.Submissions
            .Where(s => s.FormId == formId)
            .Include(s => s.Answers)
            .ThenInclude(a => a.SelectedOption)
            .Include(s => s.Answers)
            .ThenInclude(a => a.Question)
            .ToListAsync(cancellationToken);
    }
}