namespace VisitEmAll.Models;

public class HolidayDay
{
    public int Id { get; set; }
    public int HolidayId { get; set; }
    public Holiday Holiday { get; set; } = null!;
    public DateOnly Date { get; set; }
    public ICollection<DayItem> TimelineItems { get; set; } = new List<DayItem>();
}