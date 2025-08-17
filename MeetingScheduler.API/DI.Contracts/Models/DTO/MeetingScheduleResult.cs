namespace DI.Contracts.Models.DTO
{
    public class MeetingScheduleResult
    {
        public bool Success { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ErrorMessage { get; set; }
    }
}
