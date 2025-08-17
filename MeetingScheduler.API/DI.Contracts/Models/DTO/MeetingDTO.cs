namespace DI.Contracts.Models.DTO
{
    public class MeetingDto
    {
        public List<int> ParticipantId { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime EarliestStart { get; set; }
        public DateTime LatestEnd { get; set; }
    }
}
