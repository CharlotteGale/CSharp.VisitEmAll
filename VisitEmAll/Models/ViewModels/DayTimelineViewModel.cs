namespace VisitEmAll.ViewModels;

public class DayTimelineItemViewModel
{
    public int Id { get; set; }  // maps to dayitems.id
    public int DayId { get; set; } // maps to dayitems.holidaydayid
    public TimeOnly? Time { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Notes { get; set; }
}
