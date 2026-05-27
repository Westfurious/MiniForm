using MiniForm.Application.Interfaces;
using MiniForm.Data;
using MiniForm.Models;

namespace MiniForm.Infrastructure.Repositories;

public class EfSubmissionRepository : ISubmissionRepository
{
    private readonly AppDbContext _context;

    public EfSubmissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Submission> AddAsync(Submission submission, CancellationToken cancellationToken = default)
    {
        _context.Submissions.Add(submission);
        return submission;
    }
}
