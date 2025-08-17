using DI.BLL.Services;
using DI.Contracts.Models.Domain;
using DI.Contracts.Models.DTO;
using DI.DAL.Repository.Interface;
using Moq;

namespace Meetingscheduler.Tests
{
    public class UserServicesTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserServices _userServices;

        public UserServicesTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userServices = new UserServices(_mockUserRepository.Object);
        }

        #region CreateUser Tests

        [Fact]
        public void CreateUser_ValidUser_ReturnsSuccess()
        {
            // Arrange
            var userDto = new UserDTO { Name = "John Doe" };
            _mockUserRepository.Setup(x => x.CreateUser(It.IsAny<User>()))
                .Returns("User created successfully");

            // Act
            var result = _userServices.CreateUser(userDto);

            // Assert
            Assert.Equal("User created successfully", result);
            _mockUserRepository.Verify(x => x.CreateUser(It.Is<User>(u => u.Name == "John Doe")), Times.Once);
        }

        [Fact]
        public void CreateUser_EmptyName_ReturnsError()
        {
            // Arrange
            var userDto = new UserDTO { Name = "" };

            // Act
            var result = _userServices.CreateUser(userDto);

            // Assert
            Assert.Equal("Name empty or null", result);
            _mockUserRepository.Verify(x => x.CreateUser(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public void CreateUser_NullName_ReturnsError()
        {
            // Arrange
            var userDto = new UserDTO { Name = null };

            // Act
            var result = _userServices.CreateUser(userDto);

            // Assert
            Assert.Equal("Name empty or null", result);
            _mockUserRepository.Verify(x => x.CreateUser(It.IsAny<User>()), Times.Never);
        }

        #endregion

        #region GetUserMeetings Tests

        [Fact]
        public void GetUserMeetings_ValidUserId_ReturnsMeetings()
        {
            // Arrange
            var userId = 1;
            var expectedMeetings = new List<Meeting?>
            {
                new Meeting { Id = 1, Participants = new List<int> { 1 }, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) },
                new Meeting { Id = 2, Participants = new List<int> { 1, 2 }, StartTime = DateTime.Now.AddHours(2), EndTime = DateTime.Now.AddHours(3) }
            };

            _mockUserRepository.Setup(x => x.GetMeetings(userId))
                .Returns(expectedMeetings);

            // Act
            var result = _userServices.GetUserMeetings(userId);

            // Assert
            Assert.Equal(expectedMeetings, result);
            _mockUserRepository.Verify(x => x.GetMeetings(userId), Times.Once);
        }

        [Fact]
        public void GetUserMeetings_NegativeUserId_ReturnsNull()
        {
            // Act
            var result = _userServices.GetUserMeetings(-1);

            // Assert
            Assert.Null(result);
            _mockUserRepository.Verify(x => x.GetMeetings(It.IsAny<int>()), Times.Never);
        }

        #endregion
    }
}

