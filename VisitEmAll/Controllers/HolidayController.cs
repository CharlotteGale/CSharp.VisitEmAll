using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    // ---------------------------
    // CREATE HOLIDAY
    // ---------------------------

    [HttpGet("/holidays/create")]
    public IActionResult Create()
    {
        var vm = new CreateHolidayViewModel
        {
            Activities = new List<CreateHolidayViewModel.ActivityInput> { new() }
        };

        return View(vm);
    }

    [HttpPost("/holidays/create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateHolidayViewModel vm)
    {
        if (vm.StartDate.HasValue && vm.EndDate.HasValue &&
            vm.EndDate.Value < vm.StartDate.Value)
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

        var userId = HttpContext.Session.GetInt32("User_Id");
        if (userId == null) return RedirectToAction("Login", "Auth");

        var holiday = new Holiday
        {
            UserId = userId.Value,
            Title = vm.Title,
            Location = vm.Location,
            StartDate = vm.StartDate,
            EndDate = vm.EndDate,
            Accommodation = vm.Accommodation,
            TotalCost = vm.TotalCost,
            ThumbnailUrl = vm.ThumbnailUrl,
            HeroImageUrl = vm.HeroImageUrl,
            Days = new List<HolidayDay>()
        };

        if (vm.StartDate.HasValue && vm.EndDate.HasValue)
        {
            for (var date = vm.StartDate.Value; date <= vm.EndDate.Value; date = date.AddDays(1))
            {
                holiday.Days.Add(new HolidayDay { Date = date });
            }
        }

        _db.Holidays.Add(holiday);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Holiday created successfully!";
        return RedirectToAction("Index", "Dashboard");
    }

    // ---------------------------
    // EDIT HOLIDAY (GET)
    // ---------------------------

    [HttpGet("/holidays/{id:int}/edit")]
    public async Task<IActionResult> EditHoliday(int id)
    {
        var userId = HttpContext.Session.GetInt32("User_Id");
        if (userId == null)
            return RedirectToAction("Login", "Auth");

        var holiday = await _db.Holidays.FirstOrDefaultAsync(h => h.Id == id);
        if (holiday == null || holiday.UserId != userId)
            return NotFound();

        var vm = new CreateHolidayViewModel
        {
            Id = holiday.Id,
            Title = holiday.Title,
            Location = holiday.Location,
            StartDate = holiday.StartDate,
            EndDate = holiday.EndDate,
            Accommodation = holiday.Accommodation,
            TotalCost = holiday.TotalCost,
            ThumbnailUrl = holiday.ThumbnailUrl,
            HeroImageUrl = holiday.HeroImageUrl,

            // Ensure Activities is never null so the edit form doesn't break
            Activities = new List<CreateHolidayViewModel.ActivityInput>()
        };

        return View("Edit", vm);
    }

    // ---------------------------
    // EDIT HOLIDAY (POST)
    // ---------------------------

    [HttpPost("/holidays/{id:int}/update", Name = "UpdateHolidayRoute")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateHoliday(CreateHolidayViewModel updatedHoliday, int id)
    {
        var holiday = await _db.Holidays.FirstOrDefaultAsync(h => h.Id == id);
        if (holiday == null) return NotFound();

        var userId = HttpContext.Session.GetInt32("User_Id");
        if (userId != holiday.UserId) return Forbid();

        if (updatedHoliday.StartDate.HasValue && updatedHoliday.EndDate.HasValue &&
            updatedHoliday.EndDate.Value < updatedHoliday.StartDate.Value)
        {
            ModelState.AddModelError(nameof(updatedHoliday.EndDate),
                "End date cannot be before start date.");
        }

        if (!ModelState.IsValid)
            return View("Edit", updatedHoliday);

        holiday.Title = updatedHoliday.Title;
        holiday.Location = updatedHoliday.Location;
        holiday.StartDate = updatedHoliday.StartDate;
        holiday.EndDate = updatedHoliday.EndDate;
        holiday.Accommodation = updatedHoliday.Accommodation;
        holiday.TotalCost = updatedHoliday.TotalCost;
        holiday.ThumbnailUrl = updatedHoliday.ThumbnailUrl;
        holiday.HeroImageUrl = updatedHoliday.HeroImageUrl;

        _db.Update(holiday);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Holiday updated!";
        return Redirect($"/holidays/{holiday.Id}");
    }

    // ---------------------------
    // DELETE HOLIDAY
    // ---------------------------

    [HttpPost("/holidays/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var holiday = _db.Holidays.FirstOrDefault(h => h.Id == id);
        if (holiday == null) return NotFound();

        var userId = HttpContext.Session.GetInt32("User_Id");
        if (userId == null || userId != holiday.UserId)
            return Redirect("/");

        _db.Holidays.Remove(holiday);
        _db.SaveChanges();

        return RedirectToAction("Index", "Dashboard");
    }

    // ---------------------------
    // DETAILS (READ-ONLY)
    // ---------------------------

    [HttpGet("/holidays/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var holiday = await _db.Holidays
            .Include(h => h.Days)
                .ThenInclude(d => d.TimelineItems)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (holiday == null)
            return NotFound();

        var vm = new HolidayDetailsViewModel
        {
            HolidayId = holiday.Id,
            OwnerUserId = holiday.UserId,
            Title = holiday.Title,
            Location = holiday.Location,
            Accommodation = holiday.Accommodation,
            TotalCost = holiday.TotalCost,
            StartDate = holiday.StartDate,
            EndDate = holiday.EndDate,
            HeroImageUrl = holiday.HeroImageUrl,
            Days = holiday.Days
                .OrderBy(d => d.Date)
                .Select(d => new HolidayDayViewModel
                {
                    DayId = d.Id,
                    Date = d.Date,
                    Items = MergeAndSortItems(d)
                })
                .ToList()
        };

        return View(vm);
    }

    private List<DayTimelineItemViewModel> MergeAndSortItems(HolidayDay day)
    {
        var sorted = day.TimelineItems
            .OrderBy(i => i.Time.HasValue ? 0 : 1)
            .ThenBy(i => i.Time)
            .ToList();

        return sorted.Select(i => new DayTimelineItemViewModel
        {
            Id = i.Id,
            DayId = day.Id,
            Time = i.Time,
            Name = i.Name,
            ItemType = GetItemType(i),
            Location = i.Location,
            Notes = i.Notes
        }).ToList();
    }

    private static string GetItemType(DayItem item)
    {
        return item switch
        {
            DayActivity => "Activity",
            DayRestaurant => "Restaurant",
            DayAccommodation => "Accommodation",
            _ => "Unknown"
        };
    }
}
