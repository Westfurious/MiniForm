using MiniForm.Models;

namespace MiniForm.Models;

public class Question
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public bool IsRequired { get; set; }

    public Guid FormId { get; set; }

    public Form Form { get; set; } = null!;
}