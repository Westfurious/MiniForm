using System;
using System.Collections.Generic;

namespace MiniForm.Models
{
    public class Submission
    {
        public Guid Id { get; set; }

        public Guid FormId { get; set; }

        public Form Form { get; set; } = null!;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        // Optional anonymous identifier (email, ip, or null)
        public string? RespondentIdentifier { get; set; }

        public List<Answer> Answers { get; set; } = new();
    }

    public class Answer
    {
        public Guid Id { get; set; }

        public Guid SubmissionId { get; set; }

        public Submission Submission { get; set; } = null!;

        public Guid QuestionId { get; set; }

        public Question Question { get; set; } = null!;

        public string? AnswerText { get; set; }
        
        public Guid? SelectedOptionId { get; set; }
        
        public QuestionOption? SelectedOption { get; set; }
    }
}
