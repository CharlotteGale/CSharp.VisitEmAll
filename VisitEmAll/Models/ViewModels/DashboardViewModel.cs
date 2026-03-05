using VisitEmAll.Models;

namespace VisitEmAll.ViewModels;

public class DashboardViewModel
{
    public User User { get; set; } = null!;
    public bool IsOwnDashboard { get; set; }
    public int CurrentUserId { get; set; }
    public List<Holiday> UpcomingHolidays { get; set; } = new();
    public List<Holiday> PastHolidays { get; set; } = new();
    public List<Holiday> LikedHolidays { get; set; } = new();
    public List<int> LikedHolidayIds  { get; set; } = new();
    public TravelStatsViewModel TravelStats  { get; set; } = null!;
}

