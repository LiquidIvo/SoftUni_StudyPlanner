using System.ComponentModel.DataAnnotations;

namespace StudyPlanner.ViewModels.StudySession
{
    public class StudySessionEditInputModel : StudySessionBaseModel
    {
        [Required]
        public int Id { get; set; }
    }
}
