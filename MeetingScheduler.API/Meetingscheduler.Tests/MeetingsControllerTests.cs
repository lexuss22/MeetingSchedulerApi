using DI.BLL.Services.Interface;
using DI.Contracts.Models.DTO;
using MeetingScheduler.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Meetingscheduler.Tests
{
    public class MeetingsControllerTests
    {
        private readonly Mock<IMeetingServices> _mockMeetingServices;
        private readonly MeetingsController _controller;

        public MeetingsControllerTests()
        {
            _mockMeetingServices = new Mock<IMeetingServices>();
            _controller = new MeetingsController(_mockMeetingServices.Object);
        }

        [Fact]
        public void CreateMeeting_ValidRequest_ReturnsOkWithResult()
        {
            // Arrange
            var meetingDto = new MeetingDto
            {
                ParticipantId = new List<int> { 1, 2 },
                DurationMinutes = 60,
                EarliestStart = DateTime.Now.AddHours(1),
                LatestEnd = DateTime.Now.AddHours(5)
            };

            var expectedResult = new MeetingScheduleResult
            {
                Success = true
            };

            _mockMeetingServices.Setup(x => x.ScheduleMeeting(meetingDto))
                .Returns(expectedResult);

            // Act
            var result = _controller.CreateMeeting(meetingDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<MeetingScheduleResult>(okResult.Value);
            Assert.True(returnValue.Success);
            _mockMeetingServices.Verify(x => x.ScheduleMeeting(meetingDto), Times.Once);
        }

        [Fact]
        public void CreateMeeting_ServiceReturnsFailure_ReturnsOkWithFailureResult()
        {
            // Arrange
            var meetingDto = new MeetingDto
            {
                ParticipantId = new List<int> { 1 },
                DurationMinutes = 60,
                EarliestStart = DateTime.Now.AddHours(1),
                LatestEnd = DateTime.Now.AddHours(5)
            };

            var expectedResult = new MeetingScheduleResult
            {
                Success = false,
                ErrorMessage = "No available time slot found"
            };

            _mockMeetingServices.Setup(x => x.ScheduleMeeting(meetingDto))
                .Returns(expectedResult);

            // Act
            var result = _controller.CreateMeeting(meetingDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<MeetingScheduleResult>(okResult.Value);
            Assert.False(returnValue.Success);
            Assert.Equal("No available time slot found", returnValue.ErrorMessage);
        }

        [Fact]
        public void CreateMeeting_ServiceThrowsException_ThrowsException()
        {
            // Arrange
            var meetingDto = new MeetingDto
            {
                ParticipantId = new List<int> { 1 },
                DurationMinutes = 60,
                EarliestStart = DateTime.Now.AddHours(1),
                LatestEnd = DateTime.Now.AddHours(5)
            };

            _mockMeetingServices.Setup(x => x.ScheduleMeeting(meetingDto))
                .Throws(new Exception("Service error"));

            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _controller.CreateMeeting(meetingDto));
            Assert.Equal("Service error", exception.Message);
        }

        [Fact]
        public void CreateMeeting_NullDto_CallsServiceWithNull()
        {
            // Arrange
            MeetingDto meetingDto = null;
            var expectedResult = new MeetingScheduleResult { Success = false };

            _mockMeetingServices.Setup(x => x.ScheduleMeeting(null))
                .Returns(expectedResult);

            // Act
            var result = _controller.CreateMeeting(meetingDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            _mockMeetingServices.Verify(x => x.ScheduleMeeting(null), Times.Once);
        }
    }
}

