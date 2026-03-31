using MockQueryable;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using StudyPlanner.Data.Models;
using StudyPlanner.Data.Repositories.Interfaces;
using StudyPlanner.GCommon.Enums;
using StudyPlanner.Services.Core.Contracts;
using StudyPlanner.Services.Core.Models.StudyTask;
using StudyPlanner.Services.Core.Services;

namespace StudyPlanner.Services.Tests
{
    [TestFixture]
    public class StudyTaskServiceTests
    {
        private Mock<IRepository<StudyTask>> _taskRepoMock;
        private Mock<ICategoryService> _categoryServiceMock;
        private Mock<ISubjectService> _subjectServiceMock;
        private StudyTaskService _service;
        private Guid _userId;

        [SetUp]
        public void Setup()
        {
            _taskRepoMock = new Mock<IRepository<StudyTask>>();
            _categoryServiceMock = new Mock<ICategoryService>();
            _subjectServiceMock = new Mock<ISubjectService>();
            _service = new StudyTaskService(_taskRepoMock.Object, _categoryServiceMock.Object, _subjectServiceMock.Object);
            _userId = Guid.NewGuid();
        }

        private void SetupRepo(List<StudyTask> data)
        {
            var mock = data.BuildMock();
            _taskRepoMock.Setup(r => r.All()).Returns(mock);
        }

        private void SetupValidCategoryAndSubject(int categoryId = 1, int subjectId = 1)
        {
            _categoryServiceMock.Setup(c => c.CategoryExistsAsync(categoryId, _userId)).ReturnsAsync(true);
            _subjectServiceMock.Setup(s => s.SubjectExistsAsync(subjectId, _userId)).ReturnsAsync(true);
        }

        [Test]
        public async Task CreateTaskAsync_ValidInput_AddsTaskAndSaves()
        {
            SetupValidCategoryAndSubject();
            _taskRepoMock.Setup(r => r.AddAsync(It.IsAny<StudyTask>())).Returns(Task.CompletedTask);
            _taskRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var input = new StudyTaskCreateDTO
            {
                Title = "Study Math",
                Description = "Chapter 5",
                DueDate = new DateTime(2025, 6, 1),
                Priority = TaskPriority.High,
                Status = GCommon.Enums.TaskStatus.Pending,
                CategoryId = 1,
                SubjectId = 1
            };

            await _service.CreateTaskAsync(input, _userId);

            _taskRepoMock.Verify(r => r.AddAsync(It.Is<StudyTask>(t =>
                t.Title == "Study Math" &&
                t.CategoryId == 1 &&
                t.SubjectId == 1 &&
                t.UserId == _userId)), Times.Once);
            _taskRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void CreateTaskAsync_InvalidCategory_ThrowsArgumentException()
        {
            _categoryServiceMock.Setup(c => c.CategoryExistsAsync(1, _userId)).ReturnsAsync(false);
            _subjectServiceMock.Setup(s => s.SubjectExistsAsync(1, _userId)).ReturnsAsync(true);

            var input = new StudyTaskCreateDTO { Title = "X", CategoryId = 1, SubjectId = 1 };

            Assert.ThrowsAsync<ArgumentException>(() => _service.CreateTaskAsync(input, _userId));
            _taskRepoMock.Verify(r => r.AddAsync(It.IsAny<StudyTask>()), Times.Never);
        }

        [Test]
        public void CreateTaskAsync_InvalidSubject_ThrowsArgumentException()
        {
            _categoryServiceMock.Setup(c => c.CategoryExistsAsync(1, _userId)).ReturnsAsync(true);
            _subjectServiceMock.Setup(s => s.SubjectExistsAsync(1, _userId)).ReturnsAsync(false);

            var input = new StudyTaskCreateDTO { Title = "X", CategoryId = 1, SubjectId = 1 };

            Assert.ThrowsAsync<ArgumentException>(() => _service.CreateTaskAsync(input, _userId));
            _taskRepoMock.Verify(r => r.AddAsync(It.IsAny<StudyTask>()), Times.Never);
        }

        [Test]
        public async Task GetAllTasksAsync_NoFilters_ReturnsAllTasksForUser()
        {
            SetupRepo(new List<StudyTask>
            {
                new StudyTask { Id = 1, Title = "Task 1", UserId = _userId, Priority = TaskPriority.High,   Status =  GCommon.Enums.TaskStatus.Pending, DueDate = DateTime.Now, Category = new Category { Name = "Work",   Color = "Blue"  }, Subject = new Subject { Name = "Math",    Color = "Red"  } },
                new StudyTask { Id = 2, Title = "Task 2", UserId = _userId, Priority = TaskPriority.Low,    Status =  GCommon.Enums.TaskStatus.Pending, DueDate = DateTime.Now, Category = new Category { Name = "Health", Color = "Green" }, Subject = new Subject { Name = "Science", Color = "Blue" } },
                new StudyTask { Id = 3, Title = "Task 3", UserId = Guid.NewGuid(), Priority = TaskPriority.Low, Status =  GCommon.Enums.TaskStatus.Pending, DueDate = DateTime.Now, Category = new Category { Name = "Other", Color = "Red" }, Subject = new Subject { Name = "Other", Color = "Red" } }
            });

            var (items, totalCount) = await _service.GetAllTasksAsync(_userId, null, null, 1, 10);

            Assert.That(totalCount, Is.EqualTo(2));
            Assert.That(items.Count, Is.EqualTo(2));
            Assert.That(items.Any(t => t.Title == "Task 3"), Is.False);
        }

        [Test]
        public async Task GetAllTasksAsync_WithSearchTerm_ReturnsMatchingTasks()
        {
            SetupRepo(new List<StudyTask>
            {
                new StudyTask { Id = 1, Title = "Math Homework", UserId = _userId, Priority = TaskPriority.High, Status =  GCommon.Enums.TaskStatus.Pending, DueDate = DateTime.Now, Category = new Category { Name = "Work", Color = "Blue" }, Subject = new Subject { Name = "Math", Color = "Red" } },
                new StudyTask { Id = 2, Title = "Science Lab",   UserId = _userId, Priority = TaskPriority.Low,  Status =  GCommon.Enums.TaskStatus.Pending, DueDate = DateTime.Now, Category = new Category { Name = "Work", Color = "Blue" }, Subject = new Subject { Name = "Science", Color = "Blue" } },
            });

            var (items, totalCount) = await _service.GetAllTasksAsync(_userId, "Math", null, 1, 10);

            Assert.That(totalCount, Is.EqualTo(1));
            Assert.That(items.Single().Title, Is.EqualTo("Math Homework"));
        }

        [Test]
        public async Task GetAllTasksAsync_WithPriorityFilter_ReturnsMatchingTasks()
        {
            SetupRepo(new List<StudyTask>
            {
                new StudyTask { Id = 1, Title = "High Task", UserId = _userId, Priority = TaskPriority.High, Status =  GCommon.Enums.TaskStatus.Pending, DueDate = DateTime.Now, Category = new Category { Name = "Work", Color = "Blue" }, Subject = new Subject { Name = "Math", Color = "Red" } },
                new StudyTask { Id = 2, Title = "Low Task",  UserId = _userId, Priority = TaskPriority.Low,  Status =  GCommon.Enums.TaskStatus.Pending, DueDate = DateTime.Now, Category = new Category { Name = "Work", Color = "Blue" }, Subject = new Subject { Name = "Math", Color = "Red" } },
            });

            var (items, totalCount) = await _service.GetAllTasksAsync(_userId, null, "High", 1, 10);

            Assert.That(totalCount, Is.EqualTo(1));
            Assert.That(items.Single().Title, Is.EqualTo("High Task"));
        }

        [Test]
        public async Task GetAllTasksAsync_EmptyRepo_ReturnsEmpty()
        {
            SetupRepo(new List<StudyTask>());

            var (items, totalCount) = await _service.GetAllTasksAsync(_userId, null, null, 1, 10);

            Assert.That(totalCount, Is.EqualTo(0));
            Assert.That(items, Is.Empty);
        }

        [Test]
        public async Task GetTaskByIdAsync_ValidOwner_ReturnsDTO()
        {
            SetupRepo(new List<StudyTask>
            {
                new StudyTask { Id = 1, Title = "Math", UserId = _userId, Priority = TaskPriority.High, Status =  GCommon.Enums.TaskStatus.Pending, DueDate = DateTime.Now, StudySessions = new List<StudySession>(), Category = new Category { Name = "Work", Color = "Blue" }, Subject = new Subject { Name = "Math", Color = "Red" } }
            });

            var result = await _service.GetTaskByIdAsync(1, _userId);

            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Title, Is.EqualTo("Math"));
        }

        [Test]
        public void GetTaskByIdAsync_TaskNotFound_ThrowsKeyNotFoundException()
        {
            SetupRepo(new List<StudyTask>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetTaskByIdAsync(99, _userId));
        }

        [Test]
        public void GetTaskByIdAsync_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupRepo(new List<StudyTask>
            {
                new StudyTask { Id = 1, Title = "Math", UserId = Guid.NewGuid(), Category = new Category { Name = "Work", Color = "Blue" }, Subject = new Subject { Name = "Math", Color = "Red" }, StudySessions = new List<StudySession>() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetTaskByIdAsync(1, _userId));
        }

        [Test]
        public async Task GetStudyTaskForEditByIdAsync_ValidOwner_ReturnsEditDTO()
        {
            SetupRepo(new List<StudyTask>
            {
                new StudyTask { Id = 1, Title = "Math", UserId = _userId, Priority = TaskPriority.High, Status =  GCommon.Enums.TaskStatus.Pending, CategoryId = 1, SubjectId = 1 }
            });

            var result = await _service.GetStudyTaskForEditByIdAsync(1, _userId);

            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Title, Is.EqualTo("Math"));
            Assert.That(result.CategoryId, Is.EqualTo(1));
            Assert.That(result.SubjectId, Is.EqualTo(1));
        }

        [Test]
        public void GetStudyTaskForEditByIdAsync_TaskNotFound_ThrowsKeyNotFoundException()
        {
            SetupRepo(new List<StudyTask>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetStudyTaskForEditByIdAsync(99, _userId));
        }

        [Test]
        public void GetStudyTaskForEditByIdAsync_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupRepo(new List<StudyTask>
            {
                new StudyTask { Id = 1, UserId = Guid.NewGuid() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetStudyTaskForEditByIdAsync(1, _userId));
        }

        [Test]
        public async Task UpdateTaskAsync_ValidInput_UpdatesAndSaves()
        {
            SetupRepo(new List<StudyTask>
            {
                new StudyTask { Id = 1, Title = "Old", UserId = _userId, CategoryId = 1, SubjectId = 1, Category = new Category { Name = "Work", Color = "Blue" }, Subject = new Subject { Name = "Math", Color = "Red" } }
            });
            SetupValidCategoryAndSubject();
            _taskRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var input = new StudyTaskEditDTO { Id = 1, Title = "New", CategoryId = 1, SubjectId = 1, Priority = TaskPriority.High, Status = GCommon.Enums.TaskStatus.Pending };

            await _service.UpdateTaskAsync(input, _userId);

            _taskRepoMock.Verify(r => r.Update(It.Is<StudyTask>(t => t.Title == "New")), Times.Once);
            _taskRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void UpdateTaskAsync_TaskNotFound_ThrowsKeyNotFoundException()
        {
            SetupRepo(new List<StudyTask>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.UpdateTaskAsync(new StudyTaskEditDTO { Id = 99 }, _userId));

            _taskRepoMock.Verify(r => r.Update(It.IsAny<StudyTask>()), Times.Never);
        }

        [Test]
        public void UpdateTaskAsync_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupRepo(new List<StudyTask>
            {
                new StudyTask { Id = 1, UserId = Guid.NewGuid() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.UpdateTaskAsync(new StudyTaskEditDTO { Id = 1 }, _userId));

            _taskRepoMock.Verify(r => r.Update(It.IsAny<StudyTask>()), Times.Never);
        }

        [Test]
        public void UpdateTaskAsync_InvalidCategory_ThrowsArgumentException()
        {
            SetupRepo(new List<StudyTask>
            {
                new StudyTask { Id = 1, UserId = _userId, CategoryId = 1, SubjectId = 1 }
            });
            _categoryServiceMock.Setup(c => c.CategoryExistsAsync(1, _userId)).ReturnsAsync(false);
            _subjectServiceMock.Setup(s => s.SubjectExistsAsync(1, _userId)).ReturnsAsync(true);

            Assert.ThrowsAsync<ArgumentException>(() =>
                _service.UpdateTaskAsync(new StudyTaskEditDTO { Id = 1, CategoryId = 1, SubjectId = 1 }, _userId));

            _taskRepoMock.Verify(r => r.Update(It.IsAny<StudyTask>()), Times.Never);
        }

        [Test]
        public void UpdateTaskAsync_InvalidSubject_ThrowsArgumentException()
        {
            SetupRepo(new List<StudyTask>
            {
                new StudyTask { Id = 1, UserId = _userId, CategoryId = 1, SubjectId = 1 }
            });
            _categoryServiceMock.Setup(c => c.CategoryExistsAsync(1, _userId)).ReturnsAsync(true);
            _subjectServiceMock.Setup(s => s.SubjectExistsAsync(1, _userId)).ReturnsAsync(false);

            Assert.ThrowsAsync<ArgumentException>(() =>
                _service.UpdateTaskAsync(new StudyTaskEditDTO { Id = 1, CategoryId = 1, SubjectId = 1 }, _userId));

            _taskRepoMock.Verify(r => r.Update(It.IsAny<StudyTask>()), Times.Never);
        }

        [Test]
        public async Task DeleteTaskAsync_ValidOwner_DeletesAndSaves()
        {
            SetupRepo(new List<StudyTask>
            {
                new StudyTask { Id = 1, UserId = _userId }
            });
            _taskRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            await _service.DeleteTaskAsync(1, _userId);

            _taskRepoMock.Verify(r => r.Delete(It.IsAny<StudyTask>()), Times.Once);
            _taskRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteTaskAsync_TaskNotFound_ThrowsKeyNotFoundException()
        {
            SetupRepo(new List<StudyTask>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.DeleteTaskAsync(99, _userId));

            _taskRepoMock.Verify(r => r.Delete(It.IsAny<StudyTask>()), Times.Never);
        }

        [Test]
        public void DeleteTaskAsync_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupRepo(new List<StudyTask>
            {
                new StudyTask { Id = 1, UserId = Guid.NewGuid() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.DeleteTaskAsync(1, _userId));

            _taskRepoMock.Verify(r => r.Delete(It.IsAny<StudyTask>()), Times.Never);
        }

        [Test]
        public async Task GetDetailedStudyTaskByIdAsync_ValidOwner_ReturnsDTOWithSessions()
        {
            SetupRepo(new List<StudyTask>
            {
                new StudyTask
                {
                    Id = 1,
                    Title = "Math",
                    UserId = _userId,
                    Priority = TaskPriority.High,
                    Status =  GCommon.Enums.TaskStatus.Pending,
                    DueDate = DateTime.Now,
                    Category = new Category { Name = "Work", Color = "Blue" },
                    Subject = new Subject { Name = "Math", Color = "Red" },
                    StudySessions = new List<StudySession>
                    {
                        new StudySession { Id = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1), Notes = "Session 1" },
                        new StudySession { Id = 2, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(2), Notes = "Session 2" }
                    }
                }
            });

            var (dto, totalSessions) = await _service.GetDetailedStudyTaskByIdAsync(1, _userId, 1, 10);

            Assert.That(dto.Title, Is.EqualTo("Math"));
            Assert.That(totalSessions, Is.EqualTo(2));
            Assert.That(dto.StudySessions.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetDetailedStudyTaskByIdAsync_Pagination_ReturnsCorrectPage()
        {
            SetupRepo(new List<StudyTask>
            {
                new StudyTask
                {
                    Id = 1,
                    Title = "Math",
                    UserId = _userId,
                    Priority = TaskPriority.High,
                    Status = GCommon.Enums.TaskStatus.Pending,
                    DueDate = DateTime.Now,
                    Category = new Category { Name = "Work", Color = "Blue" },
                    Subject = new Subject { Name = "Math", Color = "Red" },
                    StudySessions = Enumerable.Range(1, 10)
                        .Select(i => new StudySession { Id = i, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1), Notes = $"Session {i}" })
                        .ToList()
                }
            });

            var (dto, totalSessions) = await _service.GetDetailedStudyTaskByIdAsync(1, _userId, 2, 3);

            Assert.That(totalSessions, Is.EqualTo(10));
            Assert.That(dto.StudySessions.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetDetailedStudyTaskByIdAsync_TaskNotFound_ThrowsKeyNotFoundException()
        {
            SetupRepo(new List<StudyTask>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetDetailedStudyTaskByIdAsync(99, _userId, 1, 10));
        }

        [Test]
        public void GetDetailedStudyTaskByIdAsync_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupRepo(new List<StudyTask>
            {
                new StudyTask { Id = 1, UserId = Guid.NewGuid(), Category = new Category { Name = "Work", Color = "Blue" }, Subject = new Subject { Name = "Math", Color = "Red" }, StudySessions = new List<StudySession>() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetDetailedStudyTaskByIdAsync(1, _userId, 1, 10));
        }

        [Test]
        public async Task GetDetailedStudyTaskForPDF_ValidOwner_ReturnsDTOWithSessions()
        {
            SetupRepo(new List<StudyTask>
            {
                new StudyTask
                {
                    Id = 1,
                    Title = "Math",
                    UserId = _userId,
                    Priority = TaskPriority.High,
                    Status =  GCommon.Enums.TaskStatus.Pending,
                    DueDate = DateTime.Now,
                    Category = new Category { Name = "Work", Color = "Blue" },
                    Subject = new Subject { Name = "Math", Color = "Red" },
                    StudySessions = new List<StudySession>
                    {
                        new StudySession { Id = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1), Notes = "Session 1" }
                    }
                }
            });

            var result = await _service.GetDetailedStudyTaskForPDF(1, _userId);

            Assert.That(result.Title, Is.EqualTo("Math"));
            Assert.That(result.StudySessions.Count, Is.EqualTo(1));
        }

        [Test]
        public void GetDetailedStudyTaskForPDF_TaskNotFound_ThrowsKeyNotFoundException()
        {
            SetupRepo(new List<StudyTask>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetDetailedStudyTaskForPDF(99, _userId));
        }

        [Test]
        public void GetDetailedStudyTaskForPDF_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupRepo(new List<StudyTask>
            {
                new StudyTask { Id = 1, UserId = Guid.NewGuid(), Category = new Category { Name = "Work", Color = "Blue" }, Subject = new Subject { Name = "Math", Color = "Red" }, StudySessions = new List<StudySession>() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetDetailedStudyTaskForPDF(1, _userId));
        }
    }
}