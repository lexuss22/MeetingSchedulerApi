namespace DI.Contracts.Models.DTO
{
    public class TimeSlot
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public bool OverlapsWith(TimeSlot other)
        {
            return Start < other.End && End > other.Start;
        }

        public bool Contains(TimeSlot other)
        {
            return Start <= other.Start && End >= other.End;
        }
    }
}
