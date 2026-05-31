using System;
using System.Collections.Generic;
using System.Text;

namespace MiniForm.Models
{
    public class Form
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? Deadline { get; set; }
        public bool IsPublic { get; set; } = true;
        public bool IsAnonymous { get; set; } = false;
        public Guid? CreatedByUserId { get; set; }

        public User? CreatedByUser { get; set; }

        public List<Question> Questions { get; set; } = new();
    }
}
