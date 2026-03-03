using Microsoft.AspNetCore.Mvc;
using VisitEmAll.ViewModels;

namespace VisitEmAll.Controllers;
public class ComponentSandboxController : Controller
{
    [HttpGet("/sandbox/bento")]
    public IActionResult Bento()
    {
        var vm = new DashboardViewModel
        {
            TripsCount = 6,
            CountriesVisited = 4,
            ContinentsVisited = 2,
            WorldPercent = 2.1,
            TotalTravelDays = 42,
            LongestTripDays = 14,
            TripsThisYear = 3,
            DaysUntilNextTrip = 12,
            MostVisitedCountryCode = "JP",
            UpcomingTrips = new()
            {
                new DashboardViewModel.TripCard { Id=1, Title="Japan Trip", Location="Tokyo", ThumbnailUrl=null, StartDate=null, EndDate=null },
                new DashboardViewModel.TripCard { Id=2, Title="Paris Weekend", Location="Paris", ThumbnailUrl=null, StartDate=null, EndDate=null }
            }
        };

        return View(vm);
    }
}