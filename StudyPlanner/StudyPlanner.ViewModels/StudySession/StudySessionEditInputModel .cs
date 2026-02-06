using System.ComponentModel.DataAnnotations;

namespace StudyPlanner.ViewModels.StudySession
{
    public class StudySessionEditInputModel : StudySessionInputBaseModel
    {
        [Required]
        public int Id { get; set; }
    }
}
