using Microsoft.AspNetCore.Mvc;
using VisitEmAll.ViewModels;

namespace VisitEmAll.ViewComponents;
public class TravelStatsViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(DashboardViewModel dashboard)
    {
        dashboard ??= new DashboardViewModel();

        var vm = new TravelStatsCardViewModel
        {
            TripsCount = dashboard.TripsCount,
            CountriesVisited = dashboard.CountriesVisited,
            ContinentsVisited = dashboard.ContinentsVisited,
            WorldPercent = dashboard.WorldPercent,
            TotalTravelDays = dashboard.TotalTravelDays,
            LongestTripDays = dashboard.LongestTripDays,
            TripsThisYear = dashboard.TripsThisYear,
            DaysUntilNextTrip = dashboard.DaysUntilNextTrip,
            MostVisitedCountryCode = dashboard.MostVisitedCountryCode
        };

        return Task.FromResult<IViewComponentResult>(View(vm));
    }
}