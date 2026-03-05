namespace VisitEmAll.ViewModels;

public class HolidayDayViewModel
{
    public int DayId { get; set; } // map to holidaydays.id
    public DateOnly Date { get; set; }
    public List<DayTimelineItemViewModel> Items { get; set; } = new();
}
