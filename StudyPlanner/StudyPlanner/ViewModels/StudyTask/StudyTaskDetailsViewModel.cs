namespace StudyPlanner.ViewModels.StudyTask
{
    public class StudyTaskDetailsViewModel
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

        public IEnumerable<StudySessionItemViewModel> StudySessions { get; set; }
            = new List<StudySessionItemViewModel>();
    }
}
