using MockQueryable;
using Moq;
using StudyPlanner.Data.Models;
using StudyPlanner.Data.Repositories.Interfaces;
using StudyPlanner.Services.Core.Models.Resource;
using StudyPlanner.Services.Core.Services;

namespace StudyPlanner.Services.Tests
{
    [TestFixture]
    public class ResourceServiceTests
    {
        private Mock<IRepository<Resource>> _resourceRepoMock;
        private ResourceService _service;
        private Guid _userId;

        [SetUp]
        public void Setup()
        {
            _resourceRepoMock = new Mock<IRepository<Resource>>();
            _service = new ResourceService(_resourceRepoMock.Object);
            _userId = Guid.NewGuid();
        }

        private void SetupRepo(List<Resource> data)
        {
            var mock = data.BuildMock();
            _resourceRepoMock.Setup(r => r.All()).Returns(mock);
        }

        [Test]
        public async Task CreateResourceAsync_ValidInput_AddsResourceAndSaves()
        {
            _resourceRepoMock.Setup(r => r.AddAsync(It.IsAny<Resource>())).Returns(Task.CompletedTask);
            _resourceRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var input = new ResourceCreateDTO { Title = "EF Core Docs", Url = "https://docs.microsoft.com", Description = "Official docs" };

            await _service.CreateResourceAsync(input, _userId);

            _resourceRepoMock.Verify(r => r.AddAsync(It.Is<Resource>(res =>
                res.Title == "EF Core Docs" &&
                res.Url == "https://docs.microsoft.com" &&
                res.UserId == _userId)), Times.Once);
            _resourceRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllResourcesAsync_NoSearchTerm_ReturnsAllResourcesForUser()
        {
            SetupRepo(new List<Resource>
            {
                new Resource { Id = 1, Title = "Docs",  Url = "https://a.com", UserId = _userId },
                new Resource { Id = 2, Title = "Guide", Url = "https://b.com", UserId = _userId },
                new Resource { Id = 3, Title = "Other", Url = "https://c.com", UserId = Guid.NewGuid() }
            });

            var (items, totalCount) = await _service.GetAllResourcesAsync(_userId, null, 1, 10);

            Assert.That(totalCount, Is.EqualTo(2));
            Assert.That(items.Count, Is.EqualTo(2));
            Assert.That(items.Any(r => r.Title == "Other"), Is.False);
        }

        [Test]
        public async Task GetAllResourcesAsync_WithSearchTerm_ReturnsMatchingResources()
        {
            SetupRepo(new List<Resource>
            {
                new Resource { Id = 1, Title = "EF Core Docs", Url = "https://a.com", UserId = _userId },
                new Resource { Id = 2, Title = "Azure Guide",  Url = "https://b.com", UserId = _userId },
            });

            var (items, totalCount) = await _service.GetAllResourcesAsync(_userId, "EF Core", 1, 10);

            Assert.That(totalCount, Is.EqualTo(1));
            Assert.That(items.Single().Title, Is.EqualTo("EF Core Docs"));
        }

        [Test]
        public async Task GetAllResourcesAsync_Pagination_ReturnsCorrectPage()
        {
            SetupRepo(Enumerable.Range(1, 10)
                .Select(i => new Resource { Id = i, Title = $"Resource {i}", Url = $"https://{i}.com", UserId = _userId })
                .ToList());

            var (items, totalCount) = await _service.GetAllResourcesAsync(_userId, null, 2, 3);

            Assert.That(totalCount, Is.EqualTo(10));
            Assert.That(items.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task GetAllResourcesAsync_EmptyRepo_ReturnsEmpty()
        {
            SetupRepo(new List<Resource>());

            var (items, totalCount) = await _service.GetAllResourcesAsync(_userId, null, 1, 10);

            Assert.That(totalCount, Is.EqualTo(0));
            Assert.That(items, Is.Empty);
        }

        [Test]
        public async Task GetResourceByIdAsync_ValidOwner_ReturnsDTO()
        {
            SetupRepo(new List<Resource>
            {
                new Resource { Id = 1, Title = "Docs", Url = "https://a.com", Description = "Nice", UserId = _userId }
            });

            var result = await _service.GetResourceByIdAsync(1, _userId);

            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Title, Is.EqualTo("Docs"));
            Assert.That(result.Url, Is.EqualTo("https://a.com"));
        }

        [Test]
        public void GetResourceByIdAsync_ResourceNotFound_ThrowsKeyNotFoundException()
        {
            SetupRepo(new List<Resource>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetResourceByIdAsync(99, _userId));
        }

        [Test]
        public void GetResourceByIdAsync_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupRepo(new List<Resource>
            {
                new Resource { Id = 1, Title = "Docs", UserId = Guid.NewGuid() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetResourceByIdAsync(1, _userId));
        }

        [Test]
        public async Task GetResourceByIdAsyncForEdit_ValidOwner_ReturnsEditDTO()
        {
            SetupRepo(new List<Resource>
            {
                new Resource { Id = 1, Title = "Docs", Url = "https://a.com", Description = "Nice", UserId = _userId }
            });

            var result = await _service.GetResourceByIdAsyncForEdit(1, _userId);

            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Title, Is.EqualTo("Docs"));
            Assert.That(result.Url, Is.EqualTo("https://a.com"));
        }

        [Test]
        public void GetResourceByIdAsyncForEdit_ResourceNotFound_ThrowsKeyNotFoundException()
        {
            SetupRepo(new List<Resource>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetResourceByIdAsyncForEdit(99, _userId));
        }

        [Test]
        public void GetResourceByIdAsyncForEdit_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupRepo(new List<Resource>
            {
                new Resource { Id = 1, Title = "Docs", UserId = Guid.NewGuid() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.GetResourceByIdAsyncForEdit(1, _userId));
        }

        [Test]
        public async Task UpdateResourceAsync_ValidOwner_UpdatesAndSaves()
        {
            SetupRepo(new List<Resource>
            {
                new Resource { Id = 1, Title = "Old", Url = "https://old.com", Description = "Old desc", UserId = _userId }
            });
            _resourceRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            await _service.UpdateResourceAsync(new ResourceEditDTO { Id = 1, Title = "New", Url = "https://new.com", Description = "New desc" }, _userId);

            _resourceRepoMock.Verify(r => r.Update(It.Is<Resource>(res =>
                res.Title == "New" && res.Url == "https://new.com")), Times.Once);
            _resourceRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void UpdateResourceAsync_ResourceNotFound_ThrowsKeyNotFoundException()
        {
            SetupRepo(new List<Resource>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.UpdateResourceAsync(new ResourceEditDTO { Id = 99 }, _userId));

            _resourceRepoMock.Verify(r => r.Update(It.IsAny<Resource>()), Times.Never);
        }

        [Test]
        public void UpdateResourceAsync_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupRepo(new List<Resource>
            {
                new Resource { Id = 1, Title = "Docs", UserId = Guid.NewGuid() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.UpdateResourceAsync(new ResourceEditDTO { Id = 1 }, _userId));

            _resourceRepoMock.Verify(r => r.Update(It.IsAny<Resource>()), Times.Never);
        }

        [Test]
        public async Task DeleteResourceAsync_ValidOwner_DeletesAndSaves()
        {
            SetupRepo(new List<Resource>
            {
                new Resource { Id = 1, UserId = _userId }
            });
            _resourceRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            await _service.DeleteResourceAsync(1, _userId);

            _resourceRepoMock.Verify(r => r.Delete(It.IsAny<Resource>()), Times.Once);
            _resourceRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public void DeleteResourceAsync_ResourceNotFound_ThrowsKeyNotFoundException()
        {
            SetupRepo(new List<Resource>());

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.DeleteResourceAsync(99, _userId));

            _resourceRepoMock.Verify(r => r.Delete(It.IsAny<Resource>()), Times.Never);
        }

        [Test]
        public void DeleteResourceAsync_WrongUser_ThrowsUnauthorizedAccessException()
        {
            SetupRepo(new List<Resource>
            {
                new Resource { Id = 1, UserId = Guid.NewGuid() }
            });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _service.DeleteResourceAsync(1, _userId));

            _resourceRepoMock.Verify(r => r.Delete(It.IsAny<Resource>()), Times.Never);
        }
    }
}