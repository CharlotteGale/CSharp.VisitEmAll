namespace VisitEmAll.ViewModels;

using VisitEmAll.Models;

public class TravelStatsViewModel
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

    public static TravelStatsViewModel FromHolidays(List<Holiday> holidays)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var thisYear = DateTime.UtcNow.Year;

        var upcoming = holidays
            .Where(h => h.StartDate.HasValue && h.StartDate >= today)
            .OrderBy(h => h.StartDate)
            .ToList();

        var past = holidays
            .Where(h => h.StartDate.HasValue && h.StartDate < today)
            .OrderByDescending(h => h.StartDate)
            .ToList();

        var nextTrip = upcoming.FirstOrDefault();
        var daysUntilNext = nextTrip?.StartDate.HasValue == true
            ? nextTrip.StartDate.Value.DayNumber - today.DayNumber
            : (int?)null;

        var tripDurations = holidays
            .Where(h => h.StartDate.HasValue && h.EndDate.HasValue)
            .Select(h => h.EndDate!.Value.DayNumber - h.StartDate!.Value.DayNumber)
            .ToList();

        var visitedCountryCodes = holidays
            .Where(h => h.Country != null)
            .Select(h => h.Country!.Iso2) // swap Code for whatever the property is called
            .Distinct()
            .ToHashSet();

        var mostVisitedCountryCode = holidays
            .Where(h => h.CountryId.HasValue && h.Country != null)
            .GroupBy(h => h.CountryId!.Value)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault()?.First().Country!.Iso2;

        return new TravelStatsViewModel

        {
            TripsCount = holidays.Count,
            CountriesVisited = visitedCountryCodes.Count,
            TotalTravelDays = tripDurations.Sum(),
            LongestTripDays = tripDurations.Any() ? tripDurations.Max() : 0,
            MostVisitedCountryCode = mostVisitedCountryCode,
            TripsThisYear = holidays.Count(h => h.StartDate.HasValue && h.StartDate.Value.Year == thisYear),
            DaysUntilNextTrip = daysUntilNext,
            VisitedCountryCodes = visitedCountryCodes,
            UpcomingTrips = upcoming.Select(h => new TripCard
            {
                Id = h.Id,
                Title = h.Title,
                Location = h.Location,
                CountryCode = h.Country?.Iso2,
                HeroImageUrl = h.HeroImageUrl,
                StartDate = h.StartDate,
                EndDate = h.EndDate,
            }).ToList(),
            RecentTrips = past.Select(h => new TripCard
            {
                Id = h.Id,
                Title = h.Title,
                Location = h.Location,
                CountryCode = h.Country?.Iso2,
                HeroImageUrl = h.HeroImageUrl,
                StartDate = h.StartDate,
                EndDate = h.EndDate,
            }).ToList(),
        };
    }


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