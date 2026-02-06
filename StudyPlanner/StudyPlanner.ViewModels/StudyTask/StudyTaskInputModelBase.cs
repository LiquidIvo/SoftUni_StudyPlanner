using StudyPlanner.GCommon.Enums;
using System.ComponentModel.DataAnnotations;
using static StudyPlanner.GCommon.EntityValidation;
namespace StudyPlanner.ViewModels.StudyTask
{
    public abstract class StudyTaskInputModelBase 
    {
        [Required]
        [MinLength(StudyTaskTitleMinLength)]
        [MaxLength(StudyTaskTitleMaxLength)]
        public string Title { get; set; } = null!;

        [MaxLength(StudyTaskDescriptionMaxLength)]
        public string? Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Required]
        [EnumDataType(typeof(TaskPriority))]
        public TaskPriority Priority { get; set; }

        [Required]
        [EnumDataType(typeof(GCommon.Enums.TaskStatus))]
        public GCommon.Enums.TaskStatus Status { get; set; }

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
