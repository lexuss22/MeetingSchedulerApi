using DI.Contracts.Models.Domain;
using DI.Contracts.Models.DTO;

namespace DI.DAL.Repository.Interface
{
    public interface IMeetingRepository
    {
        MeetingScheduleResult AddMeeting(Meeting meeting);
        List<Meeting> GetAllMeeting();
        List<Meeting> GetUserMeetings(int userId);
    }
}
