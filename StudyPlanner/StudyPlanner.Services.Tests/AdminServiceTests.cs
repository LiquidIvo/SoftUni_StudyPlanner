using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;
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
        public async Task GetAllUsersAsync_ReturnsAllUsers()
        {
            SetupUsers(new List<ApplicationUser>
            {
                new ApplicationUser { Id = Guid.NewGuid(), Email = "alice@test.com", FullName = "Alice" },
                new ApplicationUser { Id = Guid.NewGuid(), Email = "bob@test.com",   FullName = "Bob" }
            });

            var result = await _service.GetAllUsersAsync();

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(u => u.Email == "alice@test.com"), Is.True);
            Assert.That(result.Any(u => u.Email == "bob@test.com"), Is.True);
        }

        [Test]
        public async Task GetAllUsersAsync_EmptyUsers_ReturnsEmpty()
        {
            SetupUsers(new List<ApplicationUser>());

            var result = await _service.GetAllUsersAsync();

            Assert.That(result, Is.Empty);
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