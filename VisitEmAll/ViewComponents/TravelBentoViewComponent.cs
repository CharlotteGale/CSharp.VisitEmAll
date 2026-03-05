using Microsoft.AspNetCore.Mvc;
using VisitEmAll.ViewModels;

namespace VisitEmAll.ViewComponents;

public class TravelBentoViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(TravelStatsViewModel dashboard)
    {
        dashboard ??= new TravelStatsViewModel();
        return Task.FromResult<IViewComponentResult>(View(dashboard));
    }
}