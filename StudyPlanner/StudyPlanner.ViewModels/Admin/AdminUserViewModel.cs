using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlanner.ViewModels.Admin
{
    public class AdminUserViewModel
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
    }
}
