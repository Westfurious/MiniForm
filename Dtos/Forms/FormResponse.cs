namespace MiniForm.Dtos.Forms;

public class FormResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? Deadline { get; set; }

    public bool IsPublic { get; set; }
    
    public bool IsAnonymous { get; set; }

    public Guid? CreatedByUserId { get; set; }

    public List<QuestionResponse> Questions { get; set; } = new();
}
