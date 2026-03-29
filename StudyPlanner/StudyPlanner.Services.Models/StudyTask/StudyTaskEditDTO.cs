using StudyPlanner.GCommon.Enums;

namespace StudyPlanner.Services.Core.Models.StudyTask
{
    public class StudyTaskEditDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public GCommon.Enums.TaskStatus Status { get; set; }
        public int CategoryId { get; set; }
        public int SubjectId { get; set; }
    }
}
