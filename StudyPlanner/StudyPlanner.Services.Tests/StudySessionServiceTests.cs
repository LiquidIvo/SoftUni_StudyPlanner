using MockQueryable;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using StudyPlanner.Data.Models;
using StudyPlanner.Data.Repositories.Interfaces;
using StudyPlanner.Services.Core.Models.StudySession;
using StudyPlanner.Services.Core.Services;

namespace StudyPlanner.Services.Tests
{
    [TestFixture]
    public class StudySessionServiceTests
    {
        private Mock<IRepository<StudySession>> _sessionRepoMock;
        private Mock<IRepository<StudyTask>> _studyTaskRepoMock;
        private StudySessionService _service;
        private Guid _userId;

        [SetUp]
        public void Setup()
        {
            _sessionRepoMock = new Mock<IRepository<StudySession>>();
            _studyTaskRepoMock = new Mock<IRepository<StudyTask>>();
            _service = new StudySessionService(_sessionRepoMock.Object, _studyTaskRepoMock.Object);
            _userId = Guid.NewGuid();
        }

        private void SetupSessionRepo(List<StudySession> data)
        {
            var mock = data.BuildMock();
            _sessionRepoMock.Setup(r => r.All()).Returns(mock);
        }

        private void SetupTaskRepo(List<StudyTask> data)
        {
            var mock = data.BuildMock();
            _studyTaskRepoMock.Setup(r => r.All()).Returns(mock);
        }

        [Test]
        public async Task CheckTaskOwnershipAsync_ValidOwner_DoesNotThrow()
        {
            SetupTaskRepo(new List<StudyTask>
            {
                new StudyTask { Id = 1, UserId = _userId }
            });

            Assert.DoesNotThrowAsync(() => _service.CheckTaskOwnershipAsync(1, _userId));
        }

        [Test]
        public void CheckTaskOwnershipAsync_TaskNotFound_ThrowsKeyNotFoundException()
        {
            SetupTaskRepo(new List<StudyTask>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.CheckTaskOwnershipAsync(99, _userId));
        }

        [Test]
        public void CheckTaskOwnershipAsync_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupTaskRepo(new List<StudyTask>
            {
                new StudyTask { Id = 1, UserId = Guid.NewGuid() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.CheckTaskOwnershipAsync(1, _userId));
        }

        [Test]
        public async Task CreateStudySessionAsync_ValidInput_AddsSessionAndSaves()
        {
            SetupTaskRepo(new List<StudyTask>
            {
                new StudyTask { Id = 1, UserId = _userId }
            });

            _sessionRepoMock.Setup(r => r.AddAsync(It.IsAny<StudySession>())).Returns(Task.CompletedTask);
            _sessionRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var input = new StudySessionCreateDTO
            {
                StartTime = new DateTime(2025, 1, 1, 9, 0, 0),
                EndTime = new DateTime(2025, 1, 1, 10, 0, 0),
                Notes = "Good session"
            };

            await _service.CreateStudySessionAsync(input, 1, _userId);

            _sessionRepoMock.Verify(r => r.AddAsync(It.Is<StudySession>(s =>
                s.StudyTaskId == 1 &&
                s.UserId == _userId &&
                s.Notes == "Good session")), Times.Once);
            _sessionRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void CreateStudySessionAsync_TaskNotFound_ThrowsKeyNotFoundException()
        {
            SetupTaskRepo(new List<StudyTask>());

            var input = new StudySessionCreateDTO { StartTime = DateTime.Now, EndTime = DateTime.Now };

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.CreateStudySessionAsync(input, 99, _userId));

            _sessionRepoMock.Verify(r => r.AddAsync(It.IsAny<StudySession>()), Times.Never);
        }

        [Test]
        public void CreateStudySessionAsync_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupTaskRepo(new List<StudyTask>
            {
                new StudyTask { Id = 1, UserId = Guid.NewGuid() }
            });

            var input = new StudySessionCreateDTO { StartTime = DateTime.Now, EndTime = DateTime.Now };

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.CreateStudySessionAsync(input, 1, _userId));

            _sessionRepoMock.Verify(r => r.AddAsync(It.IsAny<StudySession>()), Times.Never);
        }

        [Test]
        public async Task GetAllStudySessionsAsync_ReturnsSessionsForUser()
        {
            SetupSessionRepo(new List<StudySession>
            {
                new StudySession { Id = 1, UserId = _userId, StudyTaskId = 1, Notes = "First" },
                new StudySession { Id = 2, UserId = _userId, StudyTaskId = 1, Notes = "Second" },
                new StudySession { Id = 3, UserId = Guid.NewGuid(), StudyTaskId = 2, Notes = "Other" }
            });

            var result = await _service.GetAllStudySessionsAsync(_userId);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(s => s.Notes == "Other"), Is.False);
        }

        [Test]
        public async Task GetAllStudySessionsAsync_EmptyRepo_ReturnsEmpty()
        {
            SetupSessionRepo(new List<StudySession>());

            var result = await _service.GetAllStudySessionsAsync(_userId);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetStudySessionByIdAsync_ValidOwner_ReturnsDTO()
        {
            SetupSessionRepo(new List<StudySession>
            {
                new StudySession { Id = 1, UserId = _userId, StudyTaskId = 1, Notes = "Test", StartTime = new DateTime(2025, 1, 1, 9, 0, 0), EndTime = new DateTime(2025, 1, 1, 10, 0, 0) }
            });

            var result = await _service.GetStudySessionByIdAsync(1, _userId);

            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Notes, Is.EqualTo("Test"));
        }

        [Test]
        public void GetStudySessionByIdAsync_SessionNotFound_ThrowsKeyNotFoundException()
        {
            SetupSessionRepo(new List<StudySession>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetStudySessionByIdAsync(99, _userId));
        }

        [Test]
        public void GetStudySessionByIdAsync_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupSessionRepo(new List<StudySession>
            {
                new StudySession { Id = 1, UserId = Guid.NewGuid() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetStudySessionByIdAsync(1, _userId));
        }

        [Test]
        public async Task GetStudySessionByIdAsyncForEdit_ValidOwner_ReturnsEditDTO()
        {
            SetupSessionRepo(new List<StudySession>
            {
                new StudySession { Id = 1, UserId = _userId, StudyTaskId = 1, Notes = "Test", StartTime = new DateTime(2025, 1, 1, 9, 0, 0), EndTime = new DateTime(2025, 1, 1, 10, 0, 0) }
            });

            var result = await _service.GetStudySessionByIdAsyncForEdit(1, _userId);

            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Notes, Is.EqualTo("Test"));
        }

        [Test]
        public void GetStudySessionByIdAsyncForEdit_SessionNotFound_ThrowsKeyNotFoundException()
        {
            SetupSessionRepo(new List<StudySession>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetStudySessionByIdAsyncForEdit(99, _userId));
        }

        [Test]
        public void GetStudySessionByIdAsyncForEdit_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupSessionRepo(new List<StudySession>
            {
                new StudySession { Id = 1, UserId = Guid.NewGuid() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetStudySessionByIdAsyncForEdit(1, _userId));
        }

        [Test]
        public async Task UpdateStudySessionAsync_ValidOwner_UpdatesAndSaves()
        {
            SetupSessionRepo(new List<StudySession>
            {
                new StudySession { Id = 1, UserId = _userId, Notes = "Old", StartTime = new DateTime(2025, 1, 1, 9, 0, 0), EndTime = new DateTime(2025, 1, 1, 10, 0, 0) }
            });
            _sessionRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var input = new StudySessionEditDTO
            {
                Id = 1,
                Notes = "New",
                StartTime = new DateTime(2025, 1, 2, 9, 0, 0),
                EndTime = new DateTime(2025, 1, 2, 11, 0, 0)
            };

            await _service.UpdateStudySessionAsync(input, _userId);

            _sessionRepoMock.Verify(r => r.Update(It.Is<StudySession>(s =>
                s.Notes == "New")), Times.Once);
            _sessionRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void UpdateStudySessionAsync_SessionNotFound_ThrowsKeyNotFoundException()
        {
            SetupSessionRepo(new List<StudySession>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.UpdateStudySessionAsync(new StudySessionEditDTO { Id = 99 }, _userId));

            _sessionRepoMock.Verify(r => r.Update(It.IsAny<StudySession>()), Times.Never);
        }

        [Test]
        public void UpdateStudySessionAsync_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupSessionRepo(new List<StudySession>
            {
                new StudySession { Id = 1, UserId = Guid.NewGuid() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.UpdateStudySessionAsync(new StudySessionEditDTO { Id = 1 }, _userId));

            _sessionRepoMock.Verify(r => r.Update(It.IsAny<StudySession>()), Times.Never);
        }

        [Test]
        public async Task DeleteStudySessionAsync_ValidOwner_DeletesAndReturnsTaskId()
        {
            SetupSessionRepo(new List<StudySession>
            {
                new StudySession { Id = 1, UserId = _userId, StudyTaskId = 5 }
            });
            _sessionRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _service.DeleteStudySessionAsync(1, _userId);

            Assert.That(result, Is.EqualTo(5));
            _sessionRepoMock.Verify(r => r.Delete(It.IsAny<StudySession>()), Times.Once);
            _sessionRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteStudySessionAsync_SessionNotFound_ThrowsKeyNotFoundException()
        {
            SetupSessionRepo(new List<StudySession>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.DeleteStudySessionAsync(99, _userId));

            _sessionRepoMock.Verify(r => r.Delete(It.IsAny<StudySession>()), Times.Never);
        }

        [Test]
        public void DeleteStudySessionAsync_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupSessionRepo(new List<StudySession>
            {
                new StudySession { Id = 1, UserId = Guid.NewGuid(), StudyTaskId = 5 }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.DeleteStudySessionAsync(1, _userId));

            _sessionRepoMock.Verify(r => r.Delete(It.IsAny<StudySession>()), Times.Never);
        }
    }
}