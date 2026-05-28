namespace MiniForm.Dtos.Forms;

public class CreateQuestionRequest
{
    public string Title { get; set; } = string.Empty;

    public bool IsRequired { get; set; }
    
    public QuestionTypeDto Type { get; set; } = QuestionTypeDto.Text;
    public List<string> Options { get; set; } = new();
}

public enum QuestionTypeDto
{
    Text = 0,
    SingleChoice = 1,
    MultipleChoice = 2
}
