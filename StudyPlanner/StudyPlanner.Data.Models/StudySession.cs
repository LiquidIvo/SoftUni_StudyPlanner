using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static StudyPlanner.GCommon.EntityValidation;


namespace StudyPlanner.Data.Models
{
    public class StudySession
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(StudyTask))]
        public int StudyTaskId { get; set; }
        public virtual StudyTask StudyTask { get; set; } = null!;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [MaxLength(StudySessionNotesMaxLength)]
        public string? Notes { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;
        public virtual IdentityUser User { get; set; } = null!;
    }
}
