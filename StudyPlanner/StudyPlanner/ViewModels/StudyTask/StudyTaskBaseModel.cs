using StudyPlanner.Enums;
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
        [EnumDataType(typeof(TaskPriority))]
        public Enums.TaskPriority Priority { get; set; }

        [Required]
        [EnumDataType(typeof(Enums.TaskStatus))]
        public Enums.TaskStatus Status { get; set; }

        [Required(ErrorMessage = "Category field is required")]
        [Display(Name = "Category")]
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Subject field is required")]
        [Display(Name = "Subject")]
        [Range(1, int.MaxValue)]
        public int SubjectId { get; set; }

       
    }
}
