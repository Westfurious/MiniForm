using MiniForm.Dtos.Forms;

namespace MiniForm.Application.Interfaces;

public interface IAnalyticsService
{
    Task<FormAnalyticsResponse> GetFormAnalyticsAsync(Guid formId, Guid requestingUserId, CancellationToken cancellationToken = default);
}