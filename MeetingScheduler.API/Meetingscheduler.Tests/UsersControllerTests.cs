using DI.BLL.Services.Interface;
using DI.Contracts.Models.Domain;
using DI.Contracts.Models.DTO;
using MeetingScheduler.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Meetingscheduler.Tests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserServices> _mockUserServices;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUserServices = new Mock<IUserServices>();
            _controller = new UsersController(_mockUserServices.Object);
        }

        #region GetUsers Tests

        [Fact]
        public void GetUsers_ValidUserId_ReturnsOkWithMeetings()
        {
            // Arrange
            var userId = 1;
            var expectedMeetings = new List<Meeting?>
            {
                new Meeting { Id = 1, Participants = new List<int> { 1 }, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) },
                new Meeting { Id = 2, Participants = new List<int> { 1, 2 }, StartTime = DateTime.Now.AddHours(2), EndTime = DateTime.Now.AddHours(3) }
            };

            _mockUserServices.Setup(x => x.GetUserMeetings(userId))
                .Returns(expectedMeetings);

            // Act
            var result = _controller.GetUsers(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Meeting?>>(okResult.Value);
            Assert.Equal(expectedMeetings, returnValue);
            _mockUserServices.Verify(x => x.GetUserMeetings(userId), Times.Once);
        }

        [Fact]
        public void GetUsers_ServiceReturnsNull_ReturnsNotFound()
        {
            // Arrange
            var userId = 999;

            _mockUserServices.Setup(x => x.GetUserMeetings(userId))
                .Returns((List<Meeting?>)null);

            // Act
            var result = _controller.GetUsers(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No meetings found for the specified user.", notFoundResult.Value);
            _mockUserServices.Verify(x => x.GetUserMeetings(userId), Times.Once);
        }

        [Fact]
        public void GetUsers_EmptyMeetingsList_ReturnsOkWithEmptyList()
        {
            // Arrange
            var userId = 1;
            var emptyMeetings = new List<Meeting?>();

            _mockUserServices.Setup(x => x.GetUserMeetings(userId))
                .Returns(emptyMeetings);

            // Act
            var result = _controller.GetUsers(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<Meeting?>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        #endregion

        #region CreateUser Tests

        [Fact]
        public void CreateUser_ValidUser_ReturnsOkWithResult()
        {
            // Arrange
            var userDto = new UserDTO { Name = "John Doe" };
            var expectedResult = "User created successfully";

            _mockUserServices.Setup(x => x.CreateUser(userDto))
                .Returns(expectedResult);

            // Act
            var result = _controller.CreateUser(userDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResult, okResult.Value);
            _mockUserServices.Verify(x => x.CreateUser(userDto), Times.Once);
        }

        [Fact]
        public void CreateUser_ServiceReturnsError_ReturnsOkWithErrorMessage()
        {
            // Arrange
            var userDto = new UserDTO { Name = "" };
            var expectedError = "Name empty or null";

            _mockUserServices.Setup(x => x.CreateUser(userDto))
                .Returns(expectedError);

            // Act
            var result = _controller.CreateUser(userDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedError, okResult.Value);
        }

        [Fact]
        public void CreateUser_NullUserDto_CallsServiceWithNull()
        {
            // Arrange
            UserDTO userDto = null;
            var expectedResult = "Error";

            _mockUserServices.Setup(x => x.CreateUser(null))
                .Returns(expectedResult);

            // Act
            var result = _controller.CreateUser(userDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockUserServices.Verify(x => x.CreateUser(null), Times.Once);
        }

        #endregion
    }
}

