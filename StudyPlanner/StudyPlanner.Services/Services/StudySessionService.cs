using Microsoft.EntityFrameworkCore;
using StudyPlanner.Data.Models;
using StudyPlanner.Data.Repositories.Interfaces;
using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.Services.Core.Models.StudySession;

namespace StudyPlanner.Services.Core.Services
{
    public class StudySessionService : IStudySessionService
    {
        private readonly IRepository<StudySession> _sessionRepo;
        private readonly IRepository<StudyTask> _studyTaskRepo;
        public StudySessionService(IRepository<StudySession> sessionRepo, IRepository<StudyTask> studyTaskRepo)
        {
            _sessionRepo = sessionRepo;
            _studyTaskRepo = studyTaskRepo;
        }

        public async Task CheckTaskOwnershipAsync(int taskId, string userId)
        {
            var task = await _studyTaskRepo.All()
            .Where(t => t.Id == taskId)
            .Select(t => new { t.UserId })
            .FirstOrDefaultAsync();

            if (task == null)
                throw new KeyNotFoundException("Task not found.");

            if (task.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");
        }
        public async Task CreateStudySessionAsync(StudySessionCreateDTO input,int studyTaskId, string userId)
        {
            var studyTask = await _studyTaskRepo.All()
               .FirstOrDefaultAsync(c => c.Id == studyTaskId);

            if (studyTask == null)
                throw new KeyNotFoundException("StudyTask not found.");

            if (studyTask.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            var studySession = new StudySession
            {
                StudyTaskId = studyTaskId,
                StartTime = input.StartTime,
                EndTime = input.EndTime,
                Notes = input.Notes,
                UserId = userId
            };

            await _sessionRepo.AddAsync(studySession);
            await _sessionRepo.SaveChangesAsync();
        }

        public async Task<int> DeleteStudySessionAsync(int id, string userId)
        {
            var session = await _sessionRepo.All()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (session == null)
                throw new KeyNotFoundException("Session not found.");

            if (session.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            _sessionRepo.Delete(session);
            await _sessionRepo.SaveChangesAsync();

            return session.StudyTaskId;
        }

        public async Task<List<StudySessionDTO>> GetAllStudySessionsAsync(string userId)
        {
            return await _sessionRepo.All()
                .Where(s => s.UserId == userId)
                .Select(s => new StudySessionDTO
                {
                    Id = s.Id,
                    StudyTaskId = s.StudyTaskId,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Notes = s.Notes
                })
                .ToListAsync();
        }

        public async Task<StudySessionDTO> GetStudySessionByIdAsync(int id, string userId)
        {
            var session = await _sessionRepo.All()
               .FirstOrDefaultAsync(c => c.Id == id);

            if (session == null)
                throw new KeyNotFoundException("Session not found.");

            if (session.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");

            return new StudySessionDTO
            {
                Id = session.Id,
                StudyTaskId = session.StudyTaskId,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                Notes = session.Notes

            };
        }

        public async Task<StudySessionEditDTO> GetStudySessionByIdAsyncForEdit(int id, string userId)
        {
            var session = await _sessionRepo.All()
               .FirstOrDefaultAsync(c => c.Id == id);

            if (session == null)
                throw new KeyNotFoundException("Session not found.");

            if (session.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");


            return new StudySessionEditDTO
            {
                Id = session.Id,
                StudyTaskId = session.StudyTaskId,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                Notes = session.Notes
            };
        }

        public async Task UpdateStudySessionAsync(StudySessionEditDTO input, string userId)
        {
            var session = await _sessionRepo.All()
               .FirstOrDefaultAsync(c => c.Id == input.Id);

            if (session == null)
                throw new KeyNotFoundException("Session not found.");

            if (session.UserId != userId)
                throw new UnauthorizedAccessException("Access denied.");


            session.Notes = input.Notes;
            session.StartTime = input.StartTime;
            session.EndTime = input.EndTime;
            


            _sessionRepo.Update(session);
            await _sessionRepo.SaveChangesAsync();
        }
    }
}
