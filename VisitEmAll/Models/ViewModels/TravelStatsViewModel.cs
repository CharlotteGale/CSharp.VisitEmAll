namespace VisitEmAll.ViewModels;

public class TravelStatsCardViewModel
{
    public int TripsCount { get; set; }
    public int CountriesVisited { get; set; }
    public int ContinentsVisited { get; set; }
    public double WorldPercent { get; set; }
    public int TotalTravelDays { get; set; }
    public int LongestTripDays { get; set; }
    public int TripsThisYear { get; set; }
    public int? DaysUntilNextTrip { get; set; }
    public string? MostVisitedCountryCode { get; set; }
}