using Microsoft.AspNetCore.Identity;
using StudyPlanner.GCommon;
using static StudyPlanner.GCommon.EntityValidation;
using System.ComponentModel.DataAnnotations;

namespace StudyPlanner.Data.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [PersonalData]
        [DataType(DataType.Text)]
        [MaxLength(FullNameMaxLength)]
        public string? FullName { get; set; } 

        [PersonalData]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

    }
}
