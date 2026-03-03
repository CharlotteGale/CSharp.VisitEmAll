using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitEmAll.ViewModels;
using VisitEmAll.Models;

namespace VisitEmAll.Controllers;

public class DashboardController : Controller
{
    private readonly VisitEmAllDbContext _context;
    public DashboardController(VisitEmAllDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("User_Id");
        if (userId == null) return RedirectToAction("Login", "Auth");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Pull trips + ISO2 code + continent from Countries table (no CountryCode column on Holiday anymore)
        var holidays = await _context.Holidays
            .Where(h => h.UserId == userId.Value)
            .OrderByDescending(h => h.StartDate)
            .Select(h => new
            {
                Trip = new DashboardViewModel.TripCard
                {
                    Id = h.Id,
                    Title = h.Title,
                    Location = h.Location,
                    CountryCode = h.Country != null ? h.Country.Iso2 : null, // ISO2
                    ThumbnailUrl = h.ThumbnailUrl,
                    StartDate = h.StartDate,
                    EndDate = h.EndDate
                },
                Continent = h.Country != null ? h.Country.Continent : null
            })
            .ToListAsync();

        var tripCards = holidays.Select(x => x.Trip).ToList();

        var upcoming = tripCards
            .Where(h => h.StartDate != null && h.StartDate >= today)
            .OrderBy(h => h.StartDate)
            .Take(5)
            .ToList();

        var recent = tripCards
            .Where(h => h.StartDate != null && h.StartDate < today)
            .OrderByDescending(h => h.StartDate)
            .Take(5)
            .ToList();

        int TripDays(DashboardViewModel.TripCard t)
        {
            if (t.StartDate == null || t.EndDate == null) return 0;
            var days = t.EndDate.Value.DayNumber - t.StartDate.Value.DayNumber + 1;
            return Math.Max(days, 0);
        }

        var totalDays = tripCards.Sum(TripDays);
        var longest = tripCards.Select(TripDays).DefaultIfEmpty(0).Max();

        var visitedCodes = tripCards
            .Select(t => t.CountryCode)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c!.ToUpperInvariant())
            .ToHashSet();

        var continents = holidays
            .Select(x => x.Continent)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c!)
            .Distinct()
            .Count();

        var nextTrip = upcoming.FirstOrDefault();
        int? daysUntil = null;
        if (nextTrip?.StartDate != null)
            daysUntil = nextTrip.StartDate.Value.DayNumber - today.DayNumber;

        var countriesVisited = visitedCodes.Count;
        var worldPercent = countriesVisited / 195.0 * 100.0;

        var mostVisitedCountryCode = tripCards
            .Select(t => t.CountryCode?.ToUpperInvariant())
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .GroupBy(c => c!)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();

        var vm = new DashboardViewModel
        {
            TripsCount = tripCards.Count,
            CountriesVisited = countriesVisited,
            ContinentsVisited = continents,
            WorldPercent = Math.Round(worldPercent, 1),
            TotalTravelDays = totalDays,
            LongestTripDays = longest,
            MostVisitedCountryCode = mostVisitedCountryCode,
            TripsThisYear = tripCards.Count(t => t.StartDate?.Year == today.Year),
            DaysUntilNextTrip = daysUntil,
            UpcomingTrips = upcoming,
            RecentTrips = recent,
            VisitedCountryCodes = visitedCodes
        };

        return View(vm);
    }
}