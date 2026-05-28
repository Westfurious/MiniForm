namespace MiniForm.Dtos.Forms;

public class FormAnalyticsResponse
{
    public Guid FormId { get; set; }
    public string FormTitle { get; set; } = string.Empty;
    public int TotalSubmissions { get; set; }
    public List<QuestionAnalytics> Questions { get; set; } = new();
}

public class QuestionAnalytics
{
    public Guid QuestionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public QuestionTypeDto Type { get; set; }
    public int TotalAnswers { get; set; }

    public List<OptionAnalytics> Options { get; set; } = new();

    public List<TextAnswerEntry> TextAnswers { get; set; } = new();
}

public class OptionAnalytics
{
    public Guid OptionId { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
    public List<string> RespondentIdentifiers { get; set; } = new(); // пусто если анонимный
}

public class TextAnswerEntry
{
    public string AnswerText { get; set; } = string.Empty;
    public string? RespondentIdentifier { get; set; }
}