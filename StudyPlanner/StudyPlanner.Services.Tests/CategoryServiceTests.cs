using MockQueryable;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using StudyPlanner.Data.Models;
using StudyPlanner.Data.Repositories.Interfaces;
using StudyPlanner.Services.Core.Models.Category;
using StudyPlanner.Services.Core.Services;

namespace StudyPlanner.Services.Tests
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private Mock<IRepository<Category>> _categoryRepoMock;
        private CategoryService _service;
        private Guid _userId;

        [SetUp]
        public void Setup()
        {
            _categoryRepoMock = new Mock<IRepository<Category>>();
            _service = new CategoryService(_categoryRepoMock.Object);
            _userId = Guid.NewGuid();
        }

        private void SetupRepo(List<Category> data)
        {
            var mock = data.BuildMock();
            _categoryRepoMock.Setup(r => r.All()).Returns(mock);
        }

        [Test]
        public async Task CreateCategoryAsync_ValidInput_AddsCategoryAndSaves()
        {
            var input = new CategoryCreateDTO { Name = "Work", Color = "Blue" };

            _categoryRepoMock.Setup(r => r.AddAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);
            _categoryRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            await _service.CreateCategoryAsync(input, _userId);

            _categoryRepoMock.Verify(r => r.AddAsync(It.Is<Category>(
                c => c.Name == "Work" && c.Color == "Blue" && c.UserId == _userId)), Times.Once);
            _categoryRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllCategoriesAsync_NoSearchTerm_ReturnsAllCategoriesForUser()
        {
            SetupRepo(new List<Category>
            {
                new Category { Id = 1, Name = "Work",   Color = "Blue",  UserId = _userId },
                new Category { Id = 2, Name = "Health", Color = "Green", UserId = _userId },
                new Category { Id = 3, Name = "Other",  Color = "Red",   UserId = Guid.NewGuid() }
            });

            var (items, totalCount) = await _service.GetAllCategoriesAsync(_userId, null, 1, 10);

            Assert.That(totalCount, Is.EqualTo(2));
            Assert.That(items.Count, Is.EqualTo(2));
            Assert.That(items.Any(i => i.Name == "Other"), Is.False);
        }

        [Test]
        public async Task GetAllCategoriesAsync_WithSearchTerm_ReturnsMatchingCategories()
        {
            SetupRepo(new List<Category>
            {
                new Category { Id = 1, Name = "Work",   Color = "Blue",  UserId = _userId },
                new Category { Id = 2, Name = "Health", Color = "Green", UserId = _userId },
            });

            var (items, totalCount) = await _service.GetAllCategoriesAsync(_userId, "Work", 1, 10);

            Assert.That(totalCount, Is.EqualTo(1));
            Assert.That(items.Single().Name, Is.EqualTo("Work"));
        }

        [Test]
        public async Task GetAllCategoriesAsync_Pagination_ReturnsCorrectPage()
        {
            SetupRepo(Enumerable.Range(1, 10)
                .Select(i => new Category { Id = i, Name = $"Category {i}", Color = "Blue", UserId = _userId })
                .ToList());

            var (items, totalCount) = await _service.GetAllCategoriesAsync(_userId, null, 2, 3);

            Assert.That(totalCount, Is.EqualTo(10));
            Assert.That(items.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task GetAllCategoriesAsync_EmptyRepo_ReturnsEmpty()
        {
            SetupRepo(new List<Category>());

            var (items, totalCount) = await _service.GetAllCategoriesAsync(_userId, null, 1, 10);

            Assert.That(totalCount, Is.EqualTo(0));
            Assert.That(items, Is.Empty);
        }

        [Test]
        public async Task GetCategoryByIdAsync_ValidOwner_ReturnsDTO()
        {
            SetupRepo(new List<Category>
            {
                new Category { Id = 1, Name = "Work", Color = "Blue", UserId = _userId }
            });

            var result = await _service.GetCategoryByIdAsync(1, _userId);

            Assert.That(result.Name, Is.EqualTo("Work"));
            Assert.That(result.Color, Is.EqualTo("Blue"));
        }

        [Test]
        public void GetCategoryByIdAsync_CategoryNotFound_ThrowsKeyNotFoundException()
        {
            SetupRepo(new List<Category>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetCategoryByIdAsync(99, _userId));
        }

        [Test]
        public void GetCategoryByIdAsync_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupRepo(new List<Category>
            {
                new Category { Id = 1, Name = "Work", UserId = Guid.NewGuid() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetCategoryByIdAsync(1, _userId));
        }

        [Test]
        public async Task GetCategoryByIdAsyncForEdit_ValidOwner_ReturnsEditDTO()
        {
            SetupRepo(new List<Category>
            {
                new Category { Id = 1, Name = "Work", Color = "Blue", UserId = _userId }
            });

            var result = await _service.GetCategoryByIdAsyncForEdit(1, _userId);

            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Work"));
            Assert.That(result.Color, Is.EqualTo("Blue"));
        }

        [Test]
        public void GetCategoryByIdAsyncForEdit_CategoryNotFound_ThrowsKeyNotFoundException()
        {
            SetupRepo(new List<Category>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetCategoryByIdAsyncForEdit(99, _userId));
        }

        [Test]
        public void GetCategoryByIdAsyncForEdit_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupRepo(new List<Category>
            {
                new Category { Id = 1, Name = "Work", UserId = Guid.NewGuid() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetCategoryByIdAsyncForEdit(1, _userId));
        }

        [Test]
        public async Task UpdateCategoryAsync_ValidOwner_UpdatesAndSaves()
        {
            SetupRepo(new List<Category>
            {
                new Category { Id = 1, Name = "Old", Color = "Blue", UserId = _userId }
            });
            _categoryRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            await _service.UpdateCategoryAsync(new CategoryEditDTO { Id = 1, Name = "New", Color = "Red" }, _userId);

            _categoryRepoMock.Verify(r => r.Update(It.Is<Category>(
                c => c.Name == "New" && c.Color == "Red")), Times.Once);
            _categoryRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void UpdateCategoryAsync_CategoryNotFound_ThrowsKeyNotFoundException()
        {
            SetupRepo(new List<Category>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.UpdateCategoryAsync(new CategoryEditDTO { Id = 99, Name = "X", Color = "X" }, _userId));

            _categoryRepoMock.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);
        }

        [Test]
        public void UpdateCategoryAsync_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupRepo(new List<Category>
            {
                new Category { Id = 1, Name = "Work", Color = "Blue", UserId = Guid.NewGuid() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.UpdateCategoryAsync(new CategoryEditDTO { Id = 1, Name = "Hacked", Color = "X" }, _userId));

            _categoryRepoMock.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);
        }

        [Test]
        public async Task DeleteCategoryAsync_NoStudyTasks_DeletesAndSaves()
        {
            SetupRepo(new List<Category>
            {
                new Category { Id = 1, UserId = _userId, StudyTasks = new List<StudyTask>() }
            });
            _categoryRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            await _service.DeleteCategoryAsync(1, _userId);

            _categoryRepoMock.Verify(r => r.Delete(It.IsAny<Category>()), Times.Once);
            _categoryRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteCategoryAsync_HasStudyTasks_ThrowsInvalidOperationException()
        {
            SetupRepo(new List<Category>
            {
                new Category { Id = 1, UserId = _userId, StudyTasks = new List<StudyTask> { new StudyTask() } }
            });

            Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.DeleteCategoryAsync(1, _userId));

            _categoryRepoMock.Verify(r => r.Delete(It.IsAny<Category>()), Times.Never);
        }

        [Test]
        public void DeleteCategoryAsync_CategoryNotFound_ThrowsKeyNotFoundException()
        {
            SetupRepo(new List<Category>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.DeleteCategoryAsync(99, _userId));
        }

        [Test]
        public void DeleteCategoryAsync_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupRepo(new List<Category>
            {
                new Category { Id = 1, UserId = Guid.NewGuid(), StudyTasks = new List<StudyTask>() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.DeleteCategoryAsync(1, _userId));

            _categoryRepoMock.Verify(r => r.Delete(It.IsAny<Category>()), Times.Never);
        }

        [Test]
        public async Task GetCategoriesForDropdownAsync_ReturnsSelectListItemsForUser()
        {
            SetupRepo(new List<Category>
            {
                new Category { Id = 1, Name = "Work",   UserId = _userId },
                new Category { Id = 2, Name = "Health", UserId = _userId },
                new Category { Id = 3, Name = "Other",  UserId = Guid.NewGuid() }
            });

            var result = await _service.GetCategoriesForDropdownAsync(_userId);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(i => i.Text == "Work"), Is.True);
            Assert.That(result.Any(i => i.Text == "Health"), Is.True);
            Assert.That(result.Any(i => i.Text == "Other"), Is.False);
        }

        [Test]
        public async Task CategoryExistsAsync_CategoryExists_ReturnsTrue()
        {
            SetupRepo(new List<Category> { new Category { Id = 1, UserId = _userId } });

            var result = await _service.CategoryExistsAsync(1, _userId);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CategoryExistsAsync_CategoryNotFound_ReturnsFalse()
        {
            SetupRepo(new List<Category>());

            var result = await _service.CategoryExistsAsync(99, _userId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task CategoryExistsAsync_WrongUser_ReturnsFalse()
        {
            SetupRepo(new List<Category> { new Category { Id = 1, UserId = Guid.NewGuid() } });

            var result = await _service.CategoryExistsAsync(1, _userId);

            Assert.That(result, Is.False);
        }
    }
}