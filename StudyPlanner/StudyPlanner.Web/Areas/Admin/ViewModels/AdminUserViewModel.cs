namespace StudyPlanner.Web.Areas.Admin.ViewModels
{
    public class AdminUserViewModel
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public int TotalTasks { get; set; }
        public int TotalSessions { get; set; }
    }
}
