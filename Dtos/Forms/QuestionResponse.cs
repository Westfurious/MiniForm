namespace MiniForm.Dtos.Forms;

public class QuestionResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool IsRequired { get; set; }
    
    public QuestionTypeDto Type { get; set; }
    
    public List<QuestionOptionResponse> Options { get; set; } = new();
}

public class QuestionOptionResponse
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
}