using StudyPlanner.Services.Core.Models.StudyTask;

namespace StudyPlanner.Services.Core.Contracts
{
    public interface IPdfService
    {
        byte[] GenerateStudyTaskPdf(StudyTaskDetailsDTO task);
    }
}
