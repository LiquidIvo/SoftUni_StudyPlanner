namespace StudyPlanner.ViewModels.StudyTask
{
    public class StudySessionItemViewModel
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
    }
}
