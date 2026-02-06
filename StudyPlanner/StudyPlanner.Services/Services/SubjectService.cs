using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;  
using StudyPlanner.Data.Models;
using StudyPlanner.Data.Repositories.Interfaces;
using StudyPlanner.Services.Contracts;
using StudyPlanner.ViewModels.Subject;



namespace StudyPlanner.Services.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly IRepository<Subject> _subjectRepo;

        public SubjectService(IRepository<Subject> subjectRepo)
        {
            _subjectRepo = subjectRepo;
        }

        public async Task CreateSubjectAsync(SubjectCreateInputModel input, string userId)
        {

            var subject = new Subject
            {
                Name = input.Name,
                Color = input.Color,
                UserId = userId
            };

            await _subjectRepo.AddAsync(subject);
            await _subjectRepo.SaveChangesAsync();
        }

        public async Task<List<SubjectViewModel>> GetAllSubjectsAsync(string userId)
        {
           
            return await _subjectRepo.All()
                .Where(s => s.UserId == userId)
                .Select(s => new SubjectViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Color = s.Color
                })
                .ToListAsync();
        }

        public async Task<SubjectViewModel> GetSubjectByIdAsync(int id, string userId)
        {

            var subject = await _subjectRepo.All()
                .Include(s => s.StudyTasks)
                .FirstOrDefaultAsync(s => s.Id == id);
                

            if (subject == null)
                throw new KeyNotFoundException("Subject not found.");
            if (subject.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            return new SubjectViewModel
            {
                Id = subject.Id,
                Name = subject.Name,
                Color = subject.Color
            };
        }

        public async Task UpdateSubjectAsync(SubjectEditInputModel input,string userId)
        {
            var subject = await _subjectRepo.All()
                .FirstOrDefaultAsync(s => s.Id == input.Id);

            if (subject == null)
                throw new KeyNotFoundException("Subject not found.");
            if (subject.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            subject.Name = input.Name;
            subject.Color = input.Color;


            _subjectRepo.Update(subject);
            await _subjectRepo.SaveChangesAsync();
        }

        public async Task DeleteSubjectAsync(int id, string userId)
        {
            var subject = await _subjectRepo.All()
               .Include(c => c.StudyTasks)
               .FirstOrDefaultAsync(s => s.Id == id);

            if (subject == null)
                throw new KeyNotFoundException("Subject not found.");
            if (subject.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");


            if (subject.StudyTasks != null && subject.StudyTasks.Any())
                throw new InvalidOperationException("Cannot delete this subject because it has associated study tasks.");

            _subjectRepo.Delete(subject);
            await _subjectRepo.SaveChangesAsync();
        }

        public async Task<List<SelectListItem>> GetSubjectsForDropdownAsync(string userId)
        {
            return await _subjectRepo.All()
                .Where(c => c.UserId == userId)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();
        }

        public async Task<bool> SubjectExistsAsync(int subjectId, string userId)
        {
            return await _subjectRepo.All()
                .AnyAsync(s => s.Id == subjectId && s.UserId == userId);
        }
    }
}