using DI.BLL.Services.Interface;
using DI.Contracts.Models.Domain;
using DI.Contracts.Models.DTO;
using DI.DAL.Repository.Interface;

namespace DI.BLL.Services
{
    public class MeetingServices : IMeetingServices
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly IUserRepository _userRepository;

        public MeetingServices(IMeetingRepository meetingRepository, IUserRepository userRepository)
        {
            _meetingRepository = meetingRepository;
            _userRepository = userRepository;
        }
        public MeetingScheduleResult ScheduleMeeting(MeetingDto request)
        {
            try
            {
                // Validate request
                var validationResult = ValidateRequest(request);
                if (!validationResult.Success)
                    return validationResult;

                // Find earliest available time slot
                var timeSlot = FindEarliestAvailableSlot(request);

                if (timeSlot == null)
                {
                    return new MeetingScheduleResult
                    {
                        Success = false,
                        ErrorMessage = "No available time slot found within the specified time range"
                    };
                }
                // Create and add the meeting
                var meeting = Mapping(request, timeSlot);

                var result = _meetingRepository.AddMeeting(meeting);
                return result;
            }
            catch (Exception ex)
            {
                return new MeetingScheduleResult
                {
                    Success = false,
                    ErrorMessage = $"Error scheduling meeting: {ex.Message}"
                };
            }
        }
        private Meeting Mapping(MeetingDto meetingDto,TimeSlot timeSlot)
        {
            return new Meeting
            {
                Participants = meetingDto.ParticipantId.ToList(),
                StartTime = timeSlot.Start,
                EndTime = timeSlot.End
            };

        }
        private MeetingScheduleResult ValidateRequest(MeetingDto request)
        {
            if (request.ParticipantId == null || !request.ParticipantId.Any())
            {
                return new MeetingScheduleResult
                {
                    Success = false,
                    ErrorMessage = "At least one participant is required"
                };
            }

            if (request.DurationMinutes <= 0)
            {
                return new MeetingScheduleResult
                {
                    Success = false,
                    ErrorMessage = "Duration must be greater than 0"
                };
            }

            if (request.EarliestStart >= request.LatestEnd)
            {
                return new MeetingScheduleResult
                {
                    Success = false,
                    ErrorMessage = "Earliest start time must be before latest end time"
                };
            }

            // Check if requested duration fits within the time window
            var availableMinutes = (request.LatestEnd - request.EarliestStart).TotalMinutes;
            if (request.DurationMinutes > availableMinutes)
            {
                return new MeetingScheduleResult
                {
                    Success = false,
                    ErrorMessage = "Meeting duration exceeds available time window"
                };
            }
            var _user = _userRepository.GetAllUsers();
            // Validate all participants exist
            var invalidParticipants = request.ParticipantId.Where(id => !_user.Any(u => u.Id == id)).ToList();
            if (invalidParticipants.Any())
            {
                return new MeetingScheduleResult
                {
                    Success = false,
                    ErrorMessage = $"Invalid participant IDs: {string.Join(", ", invalidParticipants)}"
                };
            }

            return new MeetingScheduleResult { Success = true };
        }
        private TimeSlot FindEarliestAvailableSlot(MeetingDto request)
        {
            var duration = TimeSpan.FromMinutes(request.DurationMinutes);

            // Get all existing meetings for the participants within the time range
            var participantMeetings = GetParticipantMeetingsInRange(
                request.ParticipantId,
                request.EarliestStart,
                request.LatestEnd);

            // Sort meetings by start time
            var sortedMeetings = participantMeetings.OrderBy(m => m.StartTime).ToList();

            // Try to schedule from the earliest start time
            var currentTime = request.EarliestStart;
            var latestPossibleStart = request.LatestEnd.Subtract(duration);

            while (currentTime <= latestPossibleStart)
            {
                var proposedSlot = new TimeSlot
                {
                    Start = currentTime,
                    End = currentTime.Add(duration)
                };

                // Check if this slot conflicts with any existing meeting
                var hasConflict = sortedMeetings.Any(meeting =>
                    proposedSlot.OverlapsWith(new TimeSlot
                    {
                        Start = meeting.StartTime,
                        End = meeting.EndTime
                    }));

                if (!hasConflict)
                {
                    return proposedSlot;
                }

                // Find the next possible start time after the conflicting meeting(s)
                var conflictingMeetings = sortedMeetings.Where(meeting =>
                    proposedSlot.OverlapsWith(new TimeSlot
                    {
                        Start = meeting.StartTime,
                        End = meeting.EndTime
                    })).ToList();

                if (conflictingMeetings.Any())
                {
                    // Move to the end of the latest conflicting meeting
                    var latestConflictEnd = conflictingMeetings.Max(m => m.EndTime);
                    currentTime = latestConflictEnd;
                }
                else
                {
                    // This shouldn't happen given our logic, but increment by 1 minute as fallback
                    currentTime = currentTime.AddMinutes(1);
                }
            }

            return null; // No available slot found
        }
        private List<Meeting> GetParticipantMeetingsInRange(List<int> participantIds, DateTime start, DateTime end)
        {
            var _meetings = _meetingRepository.GetAllMeeting();
            return _meetings.Where(meeting =>
                meeting.Participants.Any(pid => participantIds.Contains(pid)) &&
                meeting.StartTime < end &&
                meeting.EndTime > start
            ).ToList();
        }
    }
}
