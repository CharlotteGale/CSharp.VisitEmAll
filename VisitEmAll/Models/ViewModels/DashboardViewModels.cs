namespace VisitEmAll.ViewModels;

public class DashboardViewModel
{
    // Stats
    public int TripsCount { get; set; }
    public int CountriesVisited { get; set; }
    public int ContinentsVisited { get; set; }
    public double WorldPercent { get; set; } // 0-100
    public int TotalTravelDays { get; set; }
    public int LongestTripDays { get; set; }
    public string? MostVisitedCountryCode { get; set; }
    public int TripsThisYear { get; set; }
    public int? DaysUntilNextTrip { get; set; }
    public List<TripCard> UpcomingTrips { get; set; } = new();
    public List<TripCard> RecentTrips { get; set; } = new();
    public HashSet<string> VisitedCountryCodes { get; set; } = new();

    public class TripCard
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Location { get; set; }
        public string? CountryCode { get; set; }
        public string? HeroImageUrl { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}