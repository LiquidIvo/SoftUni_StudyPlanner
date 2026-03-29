using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlanner.Services.Core.Models.StudyTask
{
    public class StudyTaskDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string CategoryColor { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string SubjectColor { get; set; } = null!;
    }
}
