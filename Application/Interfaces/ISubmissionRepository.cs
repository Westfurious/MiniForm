using MiniForm.Models;

namespace MiniForm.Application.Interfaces;

public interface ISubmissionRepository
{
    Task<Submission> AddAsync(Submission submission, CancellationToken cancellationToken = default);
}
