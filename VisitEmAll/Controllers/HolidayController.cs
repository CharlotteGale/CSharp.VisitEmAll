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
  [Route("/holidays/create")]
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

    var userId = HttpContext.Session.GetInt32("User_Id");
    if (userId == null) userId = 1;

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
    return RedirectToAction("Index", "Dashboard");
  }

  [HttpGet("/holidays/{id:int}")]
  public async Task<IActionResult> GetHoliday(int id)
  {
    var holiday = await _db.Holidays
      .Include(h => h.Activities)
      .FirstOrDefaultAsync(h => h.Id == id);

    if (holiday == null) return NotFound();

    return View("Details", holiday);
  }
  
  [HttpGet("/holidays/{id:int}/edit")]
  public async Task<IActionResult> EditHoliday(int id)
  {
    var holiday = await _db.Holidays
      .Include(h => h.Activities)
      .FirstOrDefaultAsync(h => h.Id == id);

    return View("Edit", holiday);
  }

  [HttpPost("/holidays/{id:int}/update", Name = "UpdateHolidayRoute")]
  public async Task<IActionResult> UpdateHoliday(CreateHolidayViewModel updatedHoliday, int id)
  {
    // var userId = HttpContext.Session.GetInt32("user_id");
    // if (userId == null) return Redirect("/");

    var holiday = await _db.Holidays
      .Include(h => h.Activities)
      .FirstOrDefaultAsync(h => h.Id == id);

    if (updatedHoliday.StartDate > updatedHoliday.EndDate)
    {
      ModelState.AddModelError(nameof(updatedHoliday.EndDate),
      "End date cannot be before start date.");
    }

    if (!ModelState.IsValid)
    {
      return View("Details", holiday);
    }

    holiday.Title = updatedHoliday.Title;
    holiday.Location = updatedHoliday.Location;
    holiday.StartDate = updatedHoliday.StartDate;
    holiday.EndDate = updatedHoliday.EndDate;
    holiday.Accommodation = updatedHoliday.Accommodation;
    holiday.Cost = updatedHoliday.Cost;
    holiday.ThumbnailUrl = updatedHoliday.ThumbnailUrl;

    _db.Activities.RemoveRange(holiday.Activities);

    var newActivities = (updatedHoliday.Activities ?? new())
        .Select(a => a.Name?.Trim())
        .Where(name => !string.IsNullOrWhiteSpace(name))
        .Select(name => new Activity
        {
          HolidayId = holiday.Id,
          Name = name!
        });
    _db.Activities.AddRange(newActivities);
    await _db.SaveChangesAsync();

    return View("Details", holiday);
  }

  [HttpPost("/holidays/{id:int}/delete")]
  [ValidateAntiForgeryToken]
  public IActionResult Delete(int id)
  {
    var holiday = _db.Holidays.FirstOrDefault(h => h.Id == id);
    if (holiday == null) return NotFound();
    _db.Holidays.Remove(holiday);
    _db.SaveChanges();

    return RedirectToAction("Index", "Home");
  }

}