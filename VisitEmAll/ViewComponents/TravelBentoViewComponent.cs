using Microsoft.AspNetCore.Mvc;
using VisitEmAll.ViewModels;

namespace VisitEmAll.ViewComponents;

public class TravelBentoViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync(DashboardViewModel dashboard)
    {
        dashboard ??= new DashboardViewModel();
        return Task.FromResult<IViewComponentResult>(View(dashboard));
    }
}