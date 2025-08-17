using DI.Contracts.Models.Domain;
using DI.Contracts.Models.DTO;
using DI.DAL.Context;
using DI.DAL.Repository.Interface;

namespace DI.DAL.Repository
{
    public class MeetingRepository : IMeetingRepository
    {
        private readonly AppData _appData;

        public MeetingRepository(AppData appData)
        {
            _appData = appData;
        }

        public MeetingScheduleResult AddMeeting(Meeting meeting)
        {
            AddMeetingToList(meeting);
            return new MeetingScheduleResult
            {
                Success = true,
                StartTime = meeting.StartTime,
                EndTime = meeting.EndTime
            };
        }

        public List<Meeting> GetAllMeeting()
        {
            var meeting = _appData.Meetings;
            return meeting;
        }

        public List<Meeting> GetUserMeetings(int userId)
        {
            return _appData.Meetings.Where(m => m.Participants.Contains(userId))
                           .OrderBy(m => m.StartTime)
                           .ToList();
        }

        private void AddMeetingToList(Meeting meeting)
        {
            var newMeeting = new Meeting
            {
                Id = _appData.Meetings.Count + 1,
                StartTime = meeting.StartTime,
                EndTime = meeting.EndTime,
                DurationMinutes = meeting.DurationMinutes,
                Participants = meeting.Participants
            };
            _appData.Meetings.Add(newMeeting);
        }

        

    }
}
