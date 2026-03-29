namespace StudyPlanner.Services.Core.Models.Admin
{
    public class AdminUserDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
    }
}
