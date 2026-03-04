namespace VisitEmAll.Models;

public class TravelTip
{
    public int Id { get; set; }
    public int HolidayId { get; set; }
    public string Text { get; set; } = string.Empty;

    public Holiday? Holiday { get; set; }
}
