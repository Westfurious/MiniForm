using MiniForm.Models;

namespace MiniForm.Application.Interfaces;

public interface IAnalyticsRepository
{
    Task<List<Submission>> GetSubmissionsWithAnswersAsync(Guid formId, CancellationToken cancellationToken = default);
}