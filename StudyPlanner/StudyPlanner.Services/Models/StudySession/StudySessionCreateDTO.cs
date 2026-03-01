namespace StudyPlanner.Services.Core.Models.StudySession
{
    public class StudySessionCreateDTO
    {
        public int StudyTaskId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
    }
}
