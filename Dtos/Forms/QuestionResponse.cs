namespace MiniForm.Dtos.Forms;

public class QuestionResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool IsRequired { get; set; }
}
