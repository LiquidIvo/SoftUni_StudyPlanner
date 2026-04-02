using NUnit.Framework;
using StudyPlanner.Services.Core.Models.StudyTask;
using StudyPlanner.Services.Core.Services;

namespace StudyPlanner.Services.Tests
{
    [TestFixture]
    public class PdfServiceTests
    {
        private PdfService _service;

        [SetUp]
        public void Setup()
        {
            _service = new PdfService();
        }

        [Test]
        public void GenerateStudyTaskPdf_ValidTask_ReturnsPdfBytes()
        {
            var task = new StudyTaskDetailsDTO
            {
                Id = 1,
                Title = "Math Exam Prep",
                Description = "Review chapters 1-5",
                DueDate = new DateTime(2025, 6, 1),
                Priority = "High",
                Status = "Pending",
                Category = "Work",
                Subject = "Math",
                StudySessions = new List<StudySessionItemDTO>
                {
                    new StudySessionItemDTO
                    {
                        Id = 1,
                        StartTime = new DateTime(2025, 5, 1, 9, 0, 0),
                        EndTime = new DateTime(2025, 5, 1, 11, 0, 0),
                        Notes = "Covered algebra"
                    }
                }
            };

            var result = _service.GenerateStudyTaskPdf(task);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.GreaterThan(0));
        }

        [Test]
        public void GenerateStudyTaskPdf_NoSessions_ReturnsPdfBytes()
        {
            var task = new StudyTaskDetailsDTO
            {
                Id = 1,
                Title = "Empty Task",
                Description = "No sessions yet",
                DueDate = new DateTime(2025, 6, 1),
                Priority = "Low",
                Status = "Pending",
                Category = "Health",
                Subject = "Biology",
                StudySessions = new List<StudySessionItemDTO>()
            };

            var result = _service.GenerateStudyTaskPdf(task);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.GreaterThan(0));
        }
    }
}