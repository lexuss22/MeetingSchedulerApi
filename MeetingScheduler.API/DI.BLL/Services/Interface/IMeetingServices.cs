using DI.Contracts.Models.DTO;

namespace DI.BLL.Services.Interface
{
    public interface IMeetingServices
    {
        MeetingScheduleResult ScheduleMeeting(MeetingDto request);
    }
}
