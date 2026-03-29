using MockQueryable;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using StudyPlanner.Data.Models;

using StudyPlanner.Data.Repositories.Interfaces;
using StudyPlanner.Services.Core.Models.Subject;
using StudyPlanner.Services.Core.Services;

namespace StudyPlanner.Services.Tests
{
    [TestFixture]
    public class SubjectServiceTests
    {
        private Mock<IRepository<Subject>> _subjectRepoMock;
        private SubjectService _service;
        private Guid _userId;

        [SetUp]
        public void Setup()
        {
            _subjectRepoMock = new Mock<IRepository<Subject>>();
            _service = new SubjectService(_subjectRepoMock.Object);
            _userId = Guid.NewGuid();
        }

        private void SetupRepo(List<Subject> data)
        {
            var mock = data.BuildMock();
            _subjectRepoMock.Setup(r => r.All()).Returns(mock);
        }

        [Test]
        public async Task CreateSubjectAsync_ValidInput_AddsSubjectAndSaves()
        {
            var input = new SubjectCreateDTO { Name = "Math", Color = "Red" };

            _subjectRepoMock.Setup(r => r.AddAsync(It.IsAny<Subject>())).Returns(Task.CompletedTask);
            _subjectRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            await _service.CreateSubjectAsync(input, _userId);

            _subjectRepoMock.Verify(r => r.AddAsync(It.Is<Subject>(
                s => s.Name == "Math" && s.Color == "Red" && s.UserId == _userId)), Times.Once);
            _subjectRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllSubjectsAsync_NoSearchTerm_ReturnsAllSubjectsForUser()
        {
            SetupRepo(new List<Subject>
            {
                new Subject { Id = 1, Name = "Math",    Color = "Red",   UserId = _userId },
                new Subject { Id = 2, Name = "Science", Color = "Blue",  UserId = _userId },
                new Subject { Id = 3, Name = "History", Color = "Green", UserId = Guid.NewGuid() }
            });

            var (items, totalCount) = await _service.GetAllSubjectsAsync(_userId, null, 1, 10);

            Assert.That(totalCount, Is.EqualTo(2));
            Assert.That(items.Count, Is.EqualTo(2));
            Assert.That(items.Any(i => i.Name == "History"), Is.False);
        }

        [Test]
        public async Task GetAllSubjectsAsync_WithSearchTerm_ReturnsMatchingSubjects()
        {
            SetupRepo(new List<Subject>
            {
                new Subject { Id = 1, Name = "Math",    Color = "Red",  UserId = _userId },
                new Subject { Id = 2, Name = "Science", Color = "Blue", UserId = _userId },
            });

            var (items, totalCount) = await _service.GetAllSubjectsAsync(_userId, "Math", 1, 10);

            Assert.That(totalCount, Is.EqualTo(1));
            Assert.That(items.Single().Name, Is.EqualTo("Math"));
        }

        [Test]
        public async Task GetAllSubjectsAsync_Pagination_ReturnsCorrectPage()
        {
            SetupRepo(Enumerable.Range(1, 10)
                .Select(i => new Subject { Id = i, Name = $"Subject {i}", Color = "Red", UserId = _userId })
                .ToList());

            var (items, totalCount) = await _service.GetAllSubjectsAsync(_userId, null, 2, 3);

            Assert.That(totalCount, Is.EqualTo(10));
            Assert.That(items.Count, Is.EqualTo(3));
            Assert.That(items.First().Name, Is.EqualTo("Subject 4"));
        }

        [Test]
        public async Task GetAllSubjectsAsync_EmptyRepo_ReturnsEmpty()
        {
            SetupRepo(new List<Subject>());

            var (items, totalCount) = await _service.GetAllSubjectsAsync(_userId, null, 1, 10);

            Assert.That(totalCount, Is.EqualTo(0));
            Assert.That(items, Is.Empty);
        }

        [Test]
        public async Task GetSubjectByIdAsync_ValidOwner_ReturnsDTO()
        {
            SetupRepo(new List<Subject>
            {
                new Subject { Id = 1, Name = "Math", Color = "Red", UserId = _userId, StudyTasks = new List<StudyTask>() }
            });

            var result = await _service.GetSubjectByIdAsync(1, _userId);

            Assert.That(result.Name, Is.EqualTo("Math"));
            Assert.That(result.Color, Is.EqualTo("Red"));
        }

        [Test]
        public void GetSubjectByIdAsync_SubjectNotFound_ThrowsKeyNotFoundException()
        {
            SetupRepo(new List<Subject>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetSubjectByIdAsync(99, _userId));
        }

        [Test]
        public void GetSubjectByIdAsync_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupRepo(new List<Subject>
            {
                new Subject { Id = 1, Name = "Math", UserId = Guid.NewGuid(), StudyTasks = new List<StudyTask>() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetSubjectByIdAsync(1, _userId));
        }

        [Test]
        public async Task UpdateSubjectAsync_ValidOwner_UpdatesAndSaves()
        {
            SetupRepo(new List<Subject>
            {
                new Subject { Id = 1, Name = "Old", Color = "Blue", UserId = _userId }
            });
            _subjectRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            await _service.UpdateSubjectAsync(new SubjectEditDTO { Id = 1, Name = "New", Color = "Red" }, _userId);

            _subjectRepoMock.Verify(r => r.Update(It.Is<Subject>(
                s => s.Name == "New" && s.Color == "Red")), Times.Once);
            _subjectRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void UpdateSubjectAsync_SubjectNotFound_ThrowsKeyNotFoundException()
        {
            SetupRepo(new List<Subject>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.UpdateSubjectAsync(new SubjectEditDTO { Id = 99, Name = "X", Color = "X" }, _userId));

            _subjectRepoMock.Verify(r => r.Update(It.IsAny<Subject>()), Times.Never);
        }

        [Test]
        public void UpdateSubjectAsync_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupRepo(new List<Subject>
            {
                new Subject { Id = 1, Name = "Math", Color = "Red", UserId = Guid.NewGuid() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.UpdateSubjectAsync(new SubjectEditDTO { Id = 1, Name = "Hacked", Color = "X" }, _userId));

            _subjectRepoMock.Verify(r => r.Update(It.IsAny<Subject>()), Times.Never);
        }

        [Test]
        public async Task DeleteSubjectAsync_NoStudyTasks_DeletesAndSaves()
        {
            SetupRepo(new List<Subject>
            {
                new Subject { Id = 1, UserId = _userId, StudyTasks = new List<StudyTask>() }
            });
            _subjectRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            await _service.DeleteSubjectAsync(1, _userId);

            _subjectRepoMock.Verify(r => r.Delete(It.IsAny<Subject>()), Times.Once);
            _subjectRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteSubjectAsync_HasStudyTasks_ThrowsInvalidOperationException()
        {
            SetupRepo(new List<Subject>
            {
                new Subject { Id = 1, UserId = _userId, StudyTasks = new List<StudyTask> { new StudyTask() } }
            });

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.DeleteSubjectAsync(1, _userId));

            _subjectRepoMock.Verify(r => r.Delete(It.IsAny<Subject>()), Times.Never);
        }

        [Test]
        public void DeleteSubjectAsync_SubjectNotFound_ThrowsKeyNotFoundException()
        {
            SetupRepo(new List<Subject>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.DeleteSubjectAsync(99, _userId));
        }

        [Test]
        public void DeleteSubjectAsync_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupRepo(new List<Subject>
            {
                new Subject { Id = 1, UserId = Guid.NewGuid(), StudyTasks = new List<StudyTask>() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.DeleteSubjectAsync(1, _userId));

            _subjectRepoMock.Verify(r => r.Delete(It.IsAny<Subject>()), Times.Never);
        }

        [Test]
        public async Task SubjectExistsAsync_SubjectExists_ReturnsTrue()
        {
            SetupRepo(new List<Subject> { new Subject { Id = 1, UserId = _userId } });

            var result = await _service.SubjectExistsAsync(1, _userId);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task SubjectExistsAsync_SubjectNotFound_ReturnsFalse()
        {
            SetupRepo(new List<Subject>());

            var result = await _service.SubjectExistsAsync(99, _userId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task SubjectExistsAsync_WrongUser_ReturnsFalse()
        {
            SetupRepo(new List<Subject> { new Subject { Id = 1, UserId = Guid.NewGuid() } });

            var result = await _service.SubjectExistsAsync(1, _userId);

            Assert.That(result, Is.False);
        }
    }
}