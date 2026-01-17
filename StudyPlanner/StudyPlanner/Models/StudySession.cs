using static StudyPlanner.Common.EntityValidation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyPlanner.Models
{
    public class StudySession
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(StudyTask))]
        public int StudyTaskId { get; set; }
        public StudyTask StudyTask { get; set; } = null!;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [MaxLength(StudySessionNotesMaxLength)]
        public string? Notes { get; set; }
    }
}
