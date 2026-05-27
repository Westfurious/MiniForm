using MiniForm.Application.Interfaces;
using MiniForm.Dtos.Forms;
using MiniForm.Models;

namespace MiniForm.Infrastructure.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IFormRepository _formRepository;
    private readonly IAnalyticsRepository _analyticsRepository;

    public AnalyticsService(IFormRepository formRepository, IAnalyticsRepository analyticsRepository)
    {
        _formRepository = formRepository;
        _analyticsRepository = analyticsRepository;
    }

    public async Task<FormAnalyticsResponse> GetFormAnalyticsAsync(Guid formId, Guid requestingUserId, CancellationToken cancellationToken = default)
    {
        var form = await _formRepository.GetByIdAsync(formId, cancellationToken);
        if (form is null)
            throw new InvalidOperationException("Form not found.");

        if (form.CreatedByUserId != requestingUserId)
            throw new UnauthorizedAccessException("Only the form owner can view analytics.");

        var submissions = await _analyticsRepository.GetSubmissionsWithAnswersAsync(formId, cancellationToken);

        var response = new FormAnalyticsResponse
        {
            FormId = form.Id,
            FormTitle = form.Title,
            TotalSubmissions = submissions.Count,
            Questions = form.Questions.Select(q => BuildQuestionAnalytics(q, submissions, form.IsAnonymous)).ToList()
        };

        return response;
    }

    private static QuestionAnalytics BuildQuestionAnalytics(Question question, List<Submission> submissions, bool isAnonymous)
    {
        var allAnswers = submissions
            .SelectMany(s => s.Answers.Select(a => (Submission: s, Answer: a)))
            .Where(x => x.Answer.QuestionId == question.Id)
            .ToList();

        var analytics = new QuestionAnalytics
        {
            QuestionId = question.Id,
            Title = question.Title,
            Type = (QuestionTypeDto)question.Type,
            TotalAnswers = allAnswers.Count
        };

        if (question.Type == QuestionType.Text)
        {
            analytics.TextAnswers = allAnswers.Select(x => new TextAnswerEntry
            {
                AnswerText = x.Answer.AnswerText ?? string.Empty,
                RespondentIdentifier = isAnonymous ? null : x.Submission.RespondentIdentifier
            }).ToList();
        }
        else
        {
            analytics.Options = question.Options.OrderBy(o => o.Order).Select(option =>
            {
                var voters = allAnswers
                    .Where(x => x.Answer.SelectedOptionId == option.Id)
                    .ToList();

                return new OptionAnalytics
                {
                    OptionId = option.Id,
                    Text = option.Text,
                    Count = voters.Count,
                    Percentage = allAnswers.Count == 0 ? 0 : Math.Round((double)voters.Count / allAnswers.Count * 100, 1),
                    RespondentIdentifiers = isAnonymous
                        ? new List<string>()
                        : voters.Select(x => x.Submission.RespondentIdentifier ?? "anonymous").ToList()
                };
            }).ToList();
        }

        return analytics;
    }
}