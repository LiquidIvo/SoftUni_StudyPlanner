using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlanner.Services.Core.Models.Category
{
    public class CategoryCreateDTO
    {
        public string Name { get; set; } = null!;
        public string Color { get; set; } = null!;
    }
}
