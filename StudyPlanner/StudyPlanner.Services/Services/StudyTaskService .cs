using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data.Models;
using StudyPlanner.Data.Repositories.Interfaces;
using StudyPlanner.GCommon.Enums;
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

        public async Task CreateTaskAsync(StudyTaskCreateDTO input, Guid userId)
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

        public async Task<(List<StudyTaskDTO> Items, int TotalCount)> GetAllTasksAsync(Guid userId, string? searchTerm, string? priority, int pageNumber, int pageSize)
        {
            var query = _taskRepo.All()
            .Where(t => t.UserId == userId)
            .Include(t => t.Category)
            .Include(t => t.Subject)
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(t => t.Title.Contains(searchTerm));

            if(!string.IsNullOrWhiteSpace(priority) && Enum.TryParse<TaskPriority>(priority,out var parsedPriority))
                query = query.Where(t => t.Priority == parsedPriority);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(st => st.DueDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new StudyTaskDTO
                 {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    Priority = t.Priority.ToString(),
                    Status = t.Status.ToString(),
                    Category = t.Category.Name,
                    CategoryColor = t.Category.Color,
                    Subject = t.Subject.Name,
                    SubjectColor = t.Subject.Color
                 })
                .ToListAsync();


            return (items, totalCount);
        }

        public async Task<StudyTaskDTO> GetTaskByIdAsync(int id, Guid userId)
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

        public async Task UpdateTaskAsync(StudyTaskEditDTO input, Guid userId)
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

        public async Task DeleteTaskAsync(int id, Guid userId)
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

        public async Task<(StudyTaskDetailsDTO Task, int TotalSessions)> GetDetailedStudyTaskByIdAsync(int id, Guid userId, int pageNumber, int pageSize)
        {
            var task = await _taskRepo.All()
                .Include(t => t.Category)
                .Include(t => t.Subject)
                .Include(t => t.StudySessions)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                throw new KeyNotFoundException("Task not found.");
            if (task.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");


            var totalCount = task.StudySessions.Count();


            var sessions = task.StudySessions
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new StudySessionItemDTO
                {
                    Id = s.Id,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime, 
                    Notes = s.Notes
                }).ToList();

            var dto = new StudyTaskDetailsDTO
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
                StudySessions = sessions
            };

            return (dto, totalCount);
        }

        public async Task<StudyTaskEditDTO> GetStudyTaskForEditByIdAsync(int id, Guid userId)
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

        public async Task<StudyTaskDetailsDTO> GetDetailedStudyTaskForPDF(int id, Guid userId)
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
    }
}