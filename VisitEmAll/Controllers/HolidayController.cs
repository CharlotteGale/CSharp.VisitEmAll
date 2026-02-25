using Microsoft.AspNetCore.Mvc;
using VisitEmAll.Models;
using VisitEmAll.ViewModels;

namespace VisitEmAll.Controllers;

public class HolidaysController : Controller
{
    private readonly VisitEmAllDbContext _db;

    public HolidaysController(VisitEmAllDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Create()
    {
        var vm = new CreateHolidayViewModel
        {
            Activities = new List<CreateHolidayViewModel.ActivityInput>
            {
                new()
            }
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateHolidayViewModel vm)
    {
        if (vm.StartDate.HasValue && vm.EndDate.HasValue 
            && vm.EndDate.Value < vm.StartDate.Value)
        {
            ModelState.AddModelError(nameof(vm.EndDate), 
                "End date cannot be before start date.");
        }

        if (!ModelState.IsValid)
        {
            vm.Activities ??= new();
            if (vm.Activities.Count == 0) vm.Activities.Add(new());
            return View(vm);
        }

        var userId = HttpContext.Session.GetInt32("user_id");

        var holiday = new Holiday
        {
            UserId = userId.Value,
            Title = vm.Title,
            Location = vm.Location,
            StartDate = vm.StartDate,
            EndDate = vm.EndDate,
            Accommodation = vm.Accommodation,
            Cost = vm.Cost,
            ThumbnailUrl = vm.ThumbnailUrl
        };

        _db.Holidays.Add(holiday);
        await _db.SaveChangesAsync();

        var activityNames = (vm.Activities ?? new())
            .Select(a => a.Name?.Trim())
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToList();

        foreach (var name in activityNames)
        {
            _db.Activities.Add(new Activity
            {
                HolidayId = holiday.Id,
                Name = name!
            });
        }

        await _db.SaveChangesAsync();

        TempData["Success"] = "Holiday created successfully!";
        return RedirectToAction("Index", "Home");
    }
}