namespace DI.Contracts.Models.Domain
{
    public class Meeting
    {
        public int Id { get; set; }
        public List<int> Participants { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }
}
