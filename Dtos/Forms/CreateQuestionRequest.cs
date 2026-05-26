namespace MiniForm.Dtos.Forms;

public class CreateQuestionRequest
{
    public string Title { get; set; } = string.Empty;

    public bool IsRequired { get; set; }
}
