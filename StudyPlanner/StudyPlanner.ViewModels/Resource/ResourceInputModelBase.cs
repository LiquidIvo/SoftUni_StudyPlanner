using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StudyPlanner.GCommon.EntityValidation;

namespace StudyPlanner.ViewModels.Resource
{
    public abstract class ResourceInputModelBase
    {
        [Required]
        [MinLength(ResourceTitleMinLength)]
        [MaxLength(ResourceTitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(ResourceUrlMaxLength)]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string Url { get; set; } = null!;

        [MaxLength(ResourceDescriptionMaxLength)]
        public string? Description { get; set; }
    }
}

