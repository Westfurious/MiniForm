namespace MiniForm.Dtos.Forms;

public class CreateFormRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime? Deadline { get; set; }

    public bool IsPublic { get; set; } = true;
    
    public bool IsAnonymous { get; set; } = false;

    public List<CreateQuestionRequest> Questions { get; set; } = new();
}
