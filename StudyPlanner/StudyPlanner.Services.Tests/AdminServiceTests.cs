using Microsoft.AspNetCore.Identity;
using MockQueryable;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;
using StudyPlanner.Data.Models;
using StudyPlanner.Services.Core.Services;

namespace StudyPlanner.Services.Tests
{
    [TestFixture]
    public class AdminServiceTests
    {
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private AdminService _service;

        [SetUp]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                null!, null!, null!, null!, null!, null!, null!, null!
            );

            _service = new AdminService(_userManagerMock.Object);
        }

        private void SetupUsers(List<ApplicationUser> users)
        {
            var mock = users.BuildMock();
            _userManagerMock.Setup(m => m.Users).Returns(mock);
        }

        [Test]
        public async Task GetAllUsersAsync_NoSearchTerm_ReturnsAllUsers()
        {
            SetupUsers(new List<ApplicationUser>
            {
                new ApplicationUser { Id = Guid.NewGuid(), Email = "alice@test.com", FullName = "Alice" },
                new ApplicationUser { Id = Guid.NewGuid(), Email = "bob@test.com",   FullName = "Bob" }
            });

            var (items, totalCount) = await _service.GetAllUsersAsync(null, 1, 10);

            Assert.That(totalCount, Is.EqualTo(2));
            Assert.That(items.Count, Is.EqualTo(2));
            Assert.That(items.Any(u => u.Email == "alice@test.com"), Is.True);
            Assert.That(items.Any(u => u.Email == "bob@test.com"), Is.True);
        }

        [Test]
        public async Task GetAllUsersAsync_WithSearchTerm_FiltersByEmail()
        {
            SetupUsers(new List<ApplicationUser>
            {
                new ApplicationUser { Id = Guid.NewGuid(), Email = "alice@test.com", FullName = "Alice" },
                new ApplicationUser { Id = Guid.NewGuid(), Email = "bob@test.com",   FullName = "Bob" }
            });

            var (items, totalCount) = await _service.GetAllUsersAsync("alice", 1, 10);

            Assert.That(totalCount, Is.EqualTo(1));
            Assert.That(items.Single().Email, Is.EqualTo("alice@test.com"));
        }

        [Test]
        public async Task GetAllUsersAsync_WithSearchTerm_FiltersByFullName()
        {
            SetupUsers(new List<ApplicationUser>
            {
                new ApplicationUser { Id = Guid.NewGuid(), Email = "alice@test.com", FullName = "Alice Smith" },
                new ApplicationUser { Id = Guid.NewGuid(), Email = "bob@test.com",   FullName = "Bob Jones" }
            });

            var (items, totalCount) = await _service.GetAllUsersAsync("Alice", 1, 10);

            Assert.That(totalCount, Is.EqualTo(1));
            Assert.That(items.Single().FullName, Is.EqualTo("Alice Smith"));
        }

        [Test]
        public async Task GetAllUsersAsync_Pagination_ReturnsCorrectPage()
        {
            SetupUsers(Enumerable.Range(1, 10)
                .Select(i => new ApplicationUser { Id = Guid.NewGuid(), Email = $"user{i}@test.com", FullName = $"User {i}" })
                .ToList());

            var (items, totalCount) = await _service.GetAllUsersAsync(null, 2, 3);

            Assert.That(totalCount, Is.EqualTo(10));
            Assert.That(items.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task GetAllUsersAsync_EmptyUsers_ReturnsEmpty()
        {
            SetupUsers(new List<ApplicationUser>());

            var (items, totalCount) = await _service.GetAllUsersAsync(null, 1, 10);

            Assert.That(totalCount, Is.EqualTo(0));
            Assert.That(items, Is.Empty);
        }

        [Test]
        public async Task GetUserByIdAsync_UserExists_ReturnsDTO()
        {
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId, Email = "alice@test.com", FullName = "Alice" };

            _userManagerMock
                .Setup(m => m.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);

            var result = await _service.GetUserByIdAsync(userId);

            Assert.That(result.Email, Is.EqualTo("alice@test.com"));
            Assert.That(result.FullName, Is.EqualTo("Alice"));
        }

        [Test]
        public void GetUserByIdAsync_UserNotFound_ThrowsKeyNotFoundException()
        {
            _userManagerMock
                .Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetUserByIdAsync(Guid.NewGuid()));
        }

        [Test]
        public async Task DeleteUserAsync_UserExists_DeletesUser()
        {
            var userId = Guid.NewGuid();
            var user = new ApplicationUser { Id = userId, Email = "alice@test.com" };

            _userManagerMock
                .Setup(m => m.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
            _userManagerMock
                .Setup(m => m.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            await _service.DeleteUserAsync(userId);

            _userManagerMock.Verify(m => m.DeleteAsync(user), Times.Once);
        }

        [Test]
        public void DeleteUserAsync_UserNotFound_ThrowsKeyNotFoundException()
        {
            _userManagerMock
                .Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser?)null);

            Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.DeleteUserAsync(Guid.NewGuid()));

            _userManagerMock.Verify(m => m.DeleteAsync(It.IsAny<ApplicationUser>()), Times.Never);
        }
    }
}