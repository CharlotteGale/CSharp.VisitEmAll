namespace VisitEmAll.ViewModels;

public class DayTimelineItemViewModel
{
    public TimeOnly? Time { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Notes { get; set; }
}