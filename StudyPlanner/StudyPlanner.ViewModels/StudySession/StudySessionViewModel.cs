namespace StudyPlanner.ViewModels.StudySession
{
    public class StudySessionViewModel
    {
        public int Id { get; set; }
        public int StudyTaskId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
    }
}
