using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static StudyPlanner.GCommon.EntityValidation;


namespace StudyPlanner.Data.Models
{
    public class Resource
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ResourceTitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(ResourceUrlMaxLength)]
        public string Url { get; set; } = null!;

        [MaxLength(ResourceDescriptionMaxLength)]
        public string? Description { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
