using DI.BLL.Services;
using DI.Contracts.Models.Domain;
using DI.Contracts.Models.DTO;
using DI.DAL.Repository.Interface;
using Moq;

namespace Meetingscheduler.Tests
{
    public class MeetingServicesTests
    {
        private readonly Mock<IMeetingRepository> _mockMeetingRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly MeetingServices _meetingServices;

        public MeetingServicesTests()
        {
            _mockMeetingRepository = new Mock<IMeetingRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _meetingServices = new MeetingServices(_mockMeetingRepository.Object, _mockUserRepository.Object);
        }

        #region ScheduleMeeting Tests

        [Fact]
        public void ScheduleMeeting_ValidRequest_ReturnsSuccess()
        {
            // Arrange
            var request = new MeetingDto
            {
                ParticipantId = new List<int> { 1, 2 },
                DurationMinutes = 60,
                EarliestStart = DateTime.Now.AddHours(1),
                LatestEnd = DateTime.Now.AddHours(5)
            };

            var users = new List<User>
            {
                new User { Id = 1, Name = "User1" },
                new User { Id = 2, Name = "User2" }
            };

            _mockUserRepository.Setup(x => x.GetAllUsers()).Returns(users);
            _mockMeetingRepository.Setup(x => x.GetAllMeeting()).Returns(new List<Meeting>());
            _mockMeetingRepository.Setup(x => x.AddMeeting(It.IsAny<Meeting>()))
                .Returns(new MeetingScheduleResult { Success = true });

            // Act
            var result = _meetingServices.ScheduleMeeting(request);

            // Assert
            Assert.True(result.Success);
            _mockMeetingRepository.Verify(x => x.AddMeeting(It.IsAny<Meeting>()), Times.Once);
        }

        [Fact]
        public void ScheduleMeeting_NoParticipants_ReturnsError()
        {
            // Arrange
            var request = new MeetingDto
            {
                ParticipantId = new List<int>(),
                DurationMinutes = 60,
                EarliestStart = DateTime.Now.AddHours(1),
                LatestEnd = DateTime.Now.AddHours(5)
            };

            // Act
            var result = _meetingServices.ScheduleMeeting(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("At least one participant is required", result.ErrorMessage);
        }

        [Fact]
        public void ScheduleMeeting_NullParticipants_ReturnsError()
        {
            // Arrange
            var request = new MeetingDto
            {
                ParticipantId = null,
                DurationMinutes = 60,
                EarliestStart = DateTime.Now.AddHours(1),
                LatestEnd = DateTime.Now.AddHours(5)
            };

            // Act
            var result = _meetingServices.ScheduleMeeting(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("At least one participant is required", result.ErrorMessage);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-60)]
        public void ScheduleMeeting_InvalidDuration_ReturnsError(int durationMinutes)
        {
            // Arrange
            var request = new MeetingDto
            {
                ParticipantId = new List<int> { 1 },
                DurationMinutes = durationMinutes,
                EarliestStart = DateTime.Now.AddHours(1),
                LatestEnd = DateTime.Now.AddHours(5)
            };

            // Act
            var result = _meetingServices.ScheduleMeeting(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Duration must be greater than 0", result.ErrorMessage);
        }

        [Fact]
        public void ScheduleMeeting_EarliestStartAfterLatestEnd_ReturnsError()
        {
            // Arrange
            var baseTime = DateTime.Now;
            var request = new MeetingDto
            {
                ParticipantId = new List<int> { 1 },
                DurationMinutes = 60,
                EarliestStart = baseTime.AddHours(5),
                LatestEnd = baseTime.AddHours(1)
            };

            // Act
            var result = _meetingServices.ScheduleMeeting(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Earliest start time must be before latest end time", result.ErrorMessage);
        }

        [Fact]
        public void ScheduleMeeting_EarliestStartEqualsLatestEnd_ReturnsError()
        {
            // Arrange
            var baseTime = DateTime.Now;
            var request = new MeetingDto
            {
                ParticipantId = new List<int> { 1 },
                DurationMinutes = 60,
                EarliestStart = baseTime,
                LatestEnd = baseTime
            };

            // Act
            var result = _meetingServices.ScheduleMeeting(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Earliest start time must be before latest end time", result.ErrorMessage);
        }

        [Fact]
        public void ScheduleMeeting_DurationExceedsTimeWindow_ReturnsError()
        {
            // Arrange
            var baseTime = DateTime.Now;
            var request = new MeetingDto
            {
                ParticipantId = new List<int> { 1 },
                DurationMinutes = 120, // 2 hours
                EarliestStart = baseTime,
                LatestEnd = baseTime.AddMinutes(60) // Only 1 hour window
            };

            // Act
            var result = _meetingServices.ScheduleMeeting(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Meeting duration exceeds available time window", result.ErrorMessage);
        }

        [Fact]
        public void ScheduleMeeting_InvalidParticipantIds_ReturnsError()
        {
            // Arrange
            var request = new MeetingDto
            {
                ParticipantId = new List<int> { 1, 999 }, // 999 doesn't exist
                DurationMinutes = 60,
                EarliestStart = DateTime.Now.AddHours(1),
                LatestEnd = DateTime.Now.AddHours(5)
            };

            var users = new List<User>
            {
                new User { Id = 1, Name = "User1" }
            };

            _mockUserRepository.Setup(x => x.GetAllUsers()).Returns(users);

            // Act
            var result = _meetingServices.ScheduleMeeting(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid participant IDs: 999", result.ErrorMessage);
        }

        [Fact]
        public void ScheduleMeeting_MultipleInvalidParticipantIds_ReturnsError()
        {
            // Arrange
            var request = new MeetingDto
            {
                ParticipantId = new List<int> { 1, 888, 999 },
                DurationMinutes = 60,
                EarliestStart = DateTime.Now.AddHours(1),
                LatestEnd = DateTime.Now.AddHours(5)
            };

            var users = new List<User>
            {
                new User { Id = 1, Name = "User1" }
            };

            _mockUserRepository.Setup(x => x.GetAllUsers()).Returns(users);

            // Act
            var result = _meetingServices.ScheduleMeeting(request);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("888", result.ErrorMessage);
            Assert.Contains("999", result.ErrorMessage);
        }

        [Fact]
        public void ScheduleMeeting_NoAvailableTimeSlot_ReturnsError()
        {
            // Arrange
            var baseTime = DateTime.Now;
            var request = new MeetingDto
            {
                ParticipantId = new List<int> { 1 },
                DurationMinutes = 120, // 2 hours
                EarliestStart = baseTime,
                LatestEnd = baseTime.AddHours(3)
            };

            var users = new List<User>
            {
                new User { Id = 1, Name = "User1" }
            };

            // Existing meeting that blocks the entire time window
            var existingMeetings = new List<Meeting>
            {
                new Meeting
                {
                    Participants = new List<int> { 1 },
                    StartTime = baseTime,
                    EndTime = baseTime.AddHours(3)
                }
            };

            _mockUserRepository.Setup(x => x.GetAllUsers()).Returns(users);
            _mockMeetingRepository.Setup(x => x.GetAllMeeting()).Returns(existingMeetings);

            // Act
            var result = _meetingServices.ScheduleMeeting(request);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("No available time slot found within the specified time range", result.ErrorMessage);
        }

        [Fact]
        public void ScheduleMeeting_FindsEarliestAvailableSlot_ReturnsSuccess()
        {
            // Arrange
            var baseTime = DateTime.Now;
            var request = new MeetingDto
            {
                ParticipantId = new List<int> { 1 },
                DurationMinutes = 60,
                EarliestStart = baseTime,
                LatestEnd = baseTime.AddHours(5)
            };

            var users = new List<User>
            {
                new User { Id = 1, Name = "User1" }
            };

            // Existing meeting blocks first hour, so algorithm should find slot after it ends
            var existingMeetings = new List<Meeting>
            {
                new Meeting
                {
                    Participants = new List<int> { 1 },
                    StartTime = baseTime,
                    EndTime = baseTime.AddHours(1)
                }
            };

            _mockUserRepository.Setup(x => x.GetAllUsers()).Returns(users);
            _mockMeetingRepository.Setup(x => x.GetAllMeeting()).Returns(existingMeetings);
            _mockMeetingRepository.Setup(x => x.AddMeeting(It.IsAny<Meeting>()))
                .Returns(new MeetingScheduleResult { Success = true });

            // Act
            var result = _meetingServices.ScheduleMeeting(request);

            // Assert
            Assert.True(result.Success);
            _mockMeetingRepository.Verify(x => x.AddMeeting(It.Is<Meeting>(m =>
                m.StartTime == baseTime.AddHours(1) &&
                m.EndTime == baseTime.AddHours(2))), Times.Once);
        }

        [Fact]
        public void ScheduleMeeting_WithMultipleParticipants_ChecksAllConflicts()
        {
            // Arrange
            var baseTime = DateTime.Now;
            var request = new MeetingDto
            {
                ParticipantId = new List<int> { 1, 2 },
                DurationMinutes = 60,
                EarliestStart = baseTime,
                LatestEnd = baseTime.AddHours(5)
            };

            var users = new List<User>
            {
                new User { Id = 1, Name = "User1" },
                new User { Id = 2, Name = "User2" }
            };

            // Both users busy from 0-2 hours, so earliest slot should be at 2-3 hours
            var existingMeetings = new List<Meeting>
           {
               new Meeting
               {
                   Participants = new List<int> { 1, 2 }, // Both users in same meeting
                   StartTime = baseTime,
                   EndTime = baseTime.AddHours(2)
               }
           };

            _mockUserRepository.Setup(x => x.GetAllUsers()).Returns(users);
            _mockMeetingRepository.Setup(x => x.GetAllMeeting()).Returns(existingMeetings);
            _mockMeetingRepository.Setup(x => x.AddMeeting(It.IsAny<Meeting>()))
                .Returns(new MeetingScheduleResult { Success = true });

            // Act
            var result = _meetingServices.ScheduleMeeting(request);

            // Assert
            Assert.True(result.Success);
            _mockMeetingRepository.Verify(x => x.AddMeeting(It.Is<Meeting>(m =>
                m.StartTime == baseTime.AddHours(2) &&
                m.EndTime == baseTime.AddHours(3))), Times.Once);
        }

        [Fact]
        public void ScheduleMeeting_RepositoryThrowsException_ReturnsError()
        {
            // Arrange
            var request = new MeetingDto
            {
                ParticipantId = new List<int> { 1 },
                DurationMinutes = 60,
                EarliestStart = DateTime.Now.AddHours(1),
                LatestEnd = DateTime.Now.AddHours(5)
            };

            var users = new List<User>
            {
                new User { Id = 1, Name = "User1" }
            };

            _mockUserRepository.Setup(x => x.GetAllUsers()).Returns(users);
            _mockMeetingRepository.Setup(x => x.GetAllMeeting()).Throws(new Exception("Database error"));

            // Act
            var result = _meetingServices.ScheduleMeeting(request);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Error scheduling meeting: Database error", result.ErrorMessage);
        }

        [Fact]
        public void ScheduleMeeting_UserRepositoryThrowsException_ReturnsError()
        {
            // Arrange
            var request = new MeetingDto
            {
                ParticipantId = new List<int> { 1 },
                DurationMinutes = 60,
                EarliestStart = DateTime.Now.AddHours(1),
                LatestEnd = DateTime.Now.AddHours(5)
            };

            _mockUserRepository.Setup(x => x.GetAllUsers()).Throws(new Exception("User service error"));

            // Act
            var result = _meetingServices.ScheduleMeeting(request);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Error scheduling meeting: User service error", result.ErrorMessage);
        }

        [Fact]
        public void ScheduleMeeting_OverlappingMeetings_FindsGapBetweenMeetings()
        {
            // Arrange
            var baseTime = DateTime.Now;
            var request = new MeetingDto
            {
                ParticipantId = new List<int> { 1 },
                DurationMinutes = 30,
                EarliestStart = baseTime,
                LatestEnd = baseTime.AddHours(6)
            };

            var users = new List<User>
            {
                new User { Id = 1, Name = "User1" }
            };

            // Meetings: 9-10, 10:30-11:30, 12-13
            // Should find slot at 10:00-10:30
            var existingMeetings = new List<Meeting>
            {
                new Meeting
                {
                    Participants = new List<int> { 1 },
                    StartTime = baseTime,
                    EndTime = baseTime.AddHours(1)
                },
                new Meeting
                {
                    Participants = new List<int> { 1 },
                    StartTime = baseTime.AddMinutes(90), // 10:30
                    EndTime = baseTime.AddMinutes(150)   // 11:30
                },
                new Meeting
                {
                    Participants = new List<int> { 1 },
                    StartTime = baseTime.AddHours(3),
                    EndTime = baseTime.AddHours(4)
                }
            };

            _mockUserRepository.Setup(x => x.GetAllUsers()).Returns(users);
            _mockMeetingRepository.Setup(x => x.GetAllMeeting()).Returns(existingMeetings);
            _mockMeetingRepository.Setup(x => x.AddMeeting(It.IsAny<Meeting>()))
                .Returns(new MeetingScheduleResult { Success = true });

            // Act
            var result = _meetingServices.ScheduleMeeting(request);

            // Assert
            Assert.True(result.Success);
            _mockMeetingRepository.Verify(x => x.AddMeeting(It.Is<Meeting>(m =>
                m.StartTime == baseTime.AddHours(1) &&
                m.EndTime == baseTime.AddMinutes(90))), Times.Once);
        }

        #endregion

        #region Edge Cases and Boundary Tests

        [Fact]
        public void ScheduleMeeting_ExactFitInTimeWindow_ReturnsSuccess()
        {
            // Arrange
            var baseTime = DateTime.Now;
            var request = new MeetingDto
            {
                ParticipantId = new List<int> { 1 },
                DurationMinutes = 60,
                EarliestStart = baseTime,
                LatestEnd = baseTime.AddMinutes(60) // Exact fit
            };

            var users = new List<User>
            {
                new User { Id = 1, Name = "User1" }
            };

            _mockUserRepository.Setup(x => x.GetAllUsers()).Returns(users);
            _mockMeetingRepository.Setup(x => x.GetAllMeeting()).Returns(new List<Meeting>());
            _mockMeetingRepository.Setup(x => x.AddMeeting(It.IsAny<Meeting>()))
                .Returns(new MeetingScheduleResult { Success = true });

            // Act
            var result = _meetingServices.ScheduleMeeting(request);

            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public void ScheduleMeeting_MeetingAtEdgeOfTimeWindow_ReturnsSuccess()
        {
            // Arrange
            var baseTime = DateTime.Now;
            var request = new MeetingDto
            {
                ParticipantId = new List<int> { 1 },
                DurationMinutes = 30,
                EarliestStart = baseTime,
                LatestEnd = baseTime.AddMinutes(90)
            };

            var users = new List<User>
            {
                new User { Id = 1, Name = "User1" }
            };

            // Existing meeting takes first 30 minutes, should schedule immediately after
            var existingMeetings = new List<Meeting>
            {
                new Meeting
                {
                    Participants = new List<int> { 1 },
                    StartTime = baseTime,
                    EndTime = baseTime.AddMinutes(30)
                }
            };

            _mockUserRepository.Setup(x => x.GetAllUsers()).Returns(users);
            _mockMeetingRepository.Setup(x => x.GetAllMeeting()).Returns(existingMeetings);
            _mockMeetingRepository.Setup(x => x.AddMeeting(It.IsAny<Meeting>()))
                .Returns(new MeetingScheduleResult { Success = true });

            // Act
            var result = _meetingServices.ScheduleMeeting(request);

            // Assert
            Assert.True(result.Success);
            _mockMeetingRepository.Verify(x => x.AddMeeting(It.Is<Meeting>(m =>
                m.StartTime == baseTime.AddMinutes(30) &&
                m.EndTime == baseTime.AddMinutes(60))), Times.Once);
        }
        #endregion
    }
}

