using StudyPlanner.ViewModels.StudyTask;

namespace StudyPlanner.Services.Core.Contracts
{
    public interface IPdfService
    {
        byte[] GenerateStudyTaskPdf(StudyTaskDetailsViewModel task);
    }
}
