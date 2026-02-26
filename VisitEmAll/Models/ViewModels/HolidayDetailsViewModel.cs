namespace VisitEmAll.ViewModels;

public class HolidayDetailsViewModel
{
    public int HolidayId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Location { get; set; }
    public List<HolidayDayViewModel> Days { get; set; } = new();
}