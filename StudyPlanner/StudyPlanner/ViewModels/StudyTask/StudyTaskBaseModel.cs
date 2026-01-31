using System.ComponentModel.DataAnnotations;
using static StudyPlanner.Common.EntityValidation;
namespace StudyPlanner.ViewModels.StudyTask
{
    public abstract class StudyTaskBaseModel 
    {
        [Required]
        [MinLength(StudyTaskTitleMinLength)]
        [MaxLength(StudyTaskTitleMaxLength)]
        public string Title { get; set; } = null!;

        [MaxLength(StudyTaskDescriptionMaxLength)]
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        public Enums.TaskPriority Priority { get; set; }

        [Required]
        public Enums.TaskStatus Status { get; set; }

        [Required(ErrorMessage = "Category field is required")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Subject field is required")]
        [Display(Name = "Subject")]
        public int SubjectId { get; set; }

       
    }
}
