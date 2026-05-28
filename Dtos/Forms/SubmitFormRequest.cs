namespace MiniForm.Dtos.Forms;

public class SubmitFormRequest
{
    public List<QuestionAnswer> Answers { get; set; } = new();
}

public class QuestionAnswer
{
    public Guid QuestionId { get; set; }

    public string AnswerText { get; set; } = string.Empty;
}
