using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data.Models;
using StudyPlanner.Data.Repositories.Interfaces;
using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.Services.Core.Models.StudyTask;

namespace StudyPlanner.Services.Core.Services
{
    public class StudyTaskService : IStudyTaskService
    {
        private readonly IRepository<StudyTask> _taskRepo;
        private readonly ICategoryService _categoryService;
        private readonly ISubjectService _subjectService;

        public StudyTaskService(IRepository<StudyTask> taskRepo, ICategoryService categoryService, ISubjectService subjectService)
        {
            _taskRepo = taskRepo;
            _categoryService = categoryService;
            _subjectService = subjectService;
        }

        public async Task CreateTaskAsync(StudyTaskCreateDTO input, string userId)
        {

            if (!await _categoryService.CategoryExistsAsync(input.CategoryId, userId))
                throw new ArgumentException("Invalid category selected.");
            if (!await _subjectService.SubjectExistsAsync(input.SubjectId, userId))
                throw new ArgumentException("Invalid subject selected.");

            var task = new StudyTask
            {
                Title = input.Title,
                Description = input.Description,
                DueDate = input.DueDate,
                Priority = input.Priority,
                Status = input.Status,
                CategoryId = input.CategoryId,
                SubjectId = input.SubjectId,
                UserId = userId
            };

            await _taskRepo.AddAsync(task);
            await _taskRepo.SaveChangesAsync();
        }

        public async Task<List<StudyTaskDTO>> GetAllTasksAsync(string userId)
        {
            return await _taskRepo.All()
              .Where(s => s.UserId == userId)
              .Include(t => t.Category)
              .Include(t => t.Subject)
              .Select(s => new StudyTaskDTO
              {
                    Id = s.Id,
                    Title = s.Title.ToString(),
                    Description = s.Description,
                    DueDate = s.DueDate,
                    Priority = s.Priority.ToString(),
                    Status = s.Status.ToString(),
                    Category = s.Category.Name,
                    CategoryColor = s.Category.Color,
                    Subject = s.Subject.Name,
                    SubjectColor = s.Subject.Color
              })
              .ToListAsync();
        }

        public async Task<StudyTaskDTO> GetTaskByIdAsync(int id, string userId)
        {
            var task = await _taskRepo.All()
                .Include(t => t.StudySessions)
                .Include(c => c.Category)
                .Include(s => s.Subject)

                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                throw new KeyNotFoundException("Task not found.");
            if (task.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            return new StudyTaskDTO
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority.ToString(),
                Status = task.Status.ToString(),
                Category = task.Category.Name,
                CategoryColor = task.Category.Color,
                Subject = task.Subject.Name,
                SubjectColor = task.Subject.Color
            };
        }

        public async Task UpdateTaskAsync(StudyTaskEditDTO input, string userId)
        {
            var task = await _taskRepo
                .All()
                .FirstOrDefaultAsync(t => t.Id == input.Id);

            if (task == null)
                throw new KeyNotFoundException("Task not found.");
            if (task.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");


            if (!await _categoryService.CategoryExistsAsync(input.CategoryId, userId))
                throw new ArgumentException("Invalid category selected.");
            if (!await _subjectService.SubjectExistsAsync(input.SubjectId, userId))
                throw new ArgumentException("Invalid subject selected.");


            task.Title = input.Title;
            task.Description = input.Description;
            task.DueDate = input.DueDate;
            task.Priority = input.Priority;
            task.Status = input.Status;
            task.Category = task.Category;
            task.Subject = task.Subject;
            task.Category = task.Category;  
            task.Subject = task.Subject;


            _taskRepo.Update(task);
            await _taskRepo.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(int id, string userId)
        {
            var task = await _taskRepo
                .All()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                throw new KeyNotFoundException("Task not found.");
            if (task.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            _taskRepo.Delete(task);
            await _taskRepo.SaveChangesAsync();
        }

        public async Task<StudyTaskDetailsDTO> GetDetailedStudyTaskByIdAsync(int id, string userId)
        {
            var task = await _taskRepo
                .All()
                .Include(t => t.StudySessions)
                .Include(c => c.Category)
                .Include(s => s.Subject)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                throw new KeyNotFoundException("Task not found.");
            if (task.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            return new StudyTaskDetailsDTO
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority.ToString(),
                Status = task.Status.ToString(),
                Category = task.Category.Name,
                CategoryColor = task.Category.Color,
                Subject = task.Subject.Name,
                SubjectColor = task.Subject.Color,
                StudySessions = task.StudySessions.Select(s => new StudySessionItemDTO
                {
                    Id = s.Id,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Notes = s.Notes
                    
                }).ToList()
            };
        }

        public async Task<StudyTaskEditDTO> GetStudyTaskForEditByIdAsync(int id, string userId)
        {
            var task = await _taskRepo
                .All()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                throw new KeyNotFoundException("Task not found.");
            if (task.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            return new StudyTaskEditDTO
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Priority = task.Priority,
                Status = task.Status,
                CategoryId = task.CategoryId,
                SubjectId = task.SubjectId
            };
        }
    }
}