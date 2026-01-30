using System.ComponentModel.DataAnnotations;
using static StudyPlanner.Common.EntityValidation;
namespace StudyPlanner.ViewModels.StudySession
{
    public abstract class StudySessionBaseModel : IValidatableObject
    {
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [MaxLength(StudySessionNotesMaxLength)]
        public string? Notes { get; set; }

        [Required]
        public int StudyTaskId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartTime >= EndTime)
            {
                yield return new ValidationResult(
                    "Start time must be earlier than End time",
                    new[] { nameof(StartTime)});
            }
        }
    }
}
