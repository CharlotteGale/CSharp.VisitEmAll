using Microsoft.AspNetCore.Mvc;
using VisitEmAll.ViewModels;
using VisitEmAll.Models;
using Microsoft.EntityFrameworkCore;

namespace VisitEmAll.Controllers;

public class HomeController : Controller
{
    private readonly VisitEmAllDbContext _db;

    public HomeController(VisitEmAllDbContext db)
    {
        _db = db;
    }

    [HttpGet("/")]
    [HttpGet("/home")]
    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("User_Id");
        if (userId == null) return RedirectToAction("Login", "Auth");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Pull holidays for this user (+ Country for ISO2 / continent)
        var rows = await _db.Holidays
            .Where(h => h.UserId == userId.Value)
            .Include(h => h.Country)
            .OrderByDescending(h => h.StartDate)
            .Select(h => new
            {
                Trip = new DashboardViewModel.TripCard
                {
                    Id = h.Id,
                    Title = h.Title,
                    Location = h.Location,
                    CountryCode = h.Country != null ? h.Country.Iso2 : null,
                    HeroImageUrl = h.HeroImageUrl,
                    StartDate = h.StartDate,
                    EndDate = h.EndDate
                },
                Continent = h.Country != null ? h.Country.Continent : null
            })
            .ToListAsync();

        var trips = rows.Select(x => x.Trip).ToList();

        var upcoming = trips
            .Where(t => t.StartDate != null && t.StartDate >= today)
            .OrderBy(t => t.StartDate)
            .Take(5)
            .ToList();

        var recent = trips
            .Where(t => t.StartDate != null && t.StartDate < today)
            .OrderByDescending(t => t.StartDate)
            .Take(5)
            .ToList();

        int TripDays(DashboardViewModel.TripCard t)
        {
            if (t.StartDate == null || t.EndDate == null) return 0;
            var days = t.EndDate.Value.DayNumber - t.StartDate.Value.DayNumber + 1;
            return Math.Max(days, 0);
        }

        var visitedCodes = trips
            .Select(t => t.CountryCode)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c!.ToUpperInvariant())
            .ToHashSet();

        var continentsVisited = rows
            .Select(x => x.Continent)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c!)
            .Distinct()
            .Count();

        var nextTrip = upcoming.FirstOrDefault();
        int? daysUntil = nextTrip?.StartDate == null ? null : nextTrip.StartDate.Value.DayNumber - today.DayNumber;

        var countriesVisited = visitedCodes.Count;
        var worldPercent = countriesVisited / 195.0 * 100.0;

        var mostVisitedCountryCode = trips
            .Select(t => t.CountryCode?.ToUpperInvariant())
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .GroupBy(c => c!)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();

        var vm = new DashboardViewModel
        {
            TripsCount = trips.Count,
            CountriesVisited = countriesVisited,
            ContinentsVisited = continentsVisited,
            WorldPercent = Math.Round(worldPercent, 1),
            TotalTravelDays = trips.Sum(TripDays),
            LongestTripDays = trips.Select(TripDays).DefaultIfEmpty(0).Max(),
            MostVisitedCountryCode = mostVisitedCountryCode,
            TripsThisYear = trips.Count(t => t.StartDate?.Year == today.Year),
            DaysUntilNextTrip = daysUntil,
            UpcomingTrips = upcoming,
            RecentTrips = recent,
            VisitedCountryCodes = visitedCodes
        };

        return View(vm); // Views/Home/Index.cshtml
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
