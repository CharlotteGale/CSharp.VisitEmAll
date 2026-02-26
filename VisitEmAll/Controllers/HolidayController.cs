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
    if (userId == null) return RedirectToAction("Login", "Auth");

    var holiday = new Holiday
    {
      UserId = userId.Value,
      Title = vm.Title,
      Location = vm.Location,
      StartDate = vm.StartDate,
      EndDate = vm.EndDate,
      TotalCost = vm.TotalCost,
      ThumbnailUrl = vm.ThumbnailUrl
    };

    _db.Holidays.Add(holiday);
    await _db.SaveChangesAsync();

    var activityNames = (vm.Activities ?? new())
        .Select(a => a.Name?.Trim())
        .Where(name => !string.IsNullOrWhiteSpace(name))
        .ToList();

    // foreach (var name in activityNames)
    // {
    //   _db.Activities.Add(new Activity
    //   {
    //     HolidayId = holiday.Id,
    //     Name = name!
    //   });
    // }
    await _db.SaveChangesAsync();

    TempData["Success"] = "Holiday created successfully!";
    return RedirectToAction("Index", "Dashboard");
  }

  [HttpGet("/holidays/{id:int}")]
  public async Task<IActionResult> GetHoliday(int id)
  {
    var holiday = await _db.Holidays
      .FirstOrDefaultAsync(h => h.Id == id);

    if (holiday == null) return NotFound();

    return View("Details", holiday);
  }
  
  [HttpGet("/holidays/{id:int}/edit")]
  public async Task<IActionResult> EditHoliday(int id)
  {
    var userId = HttpContext.Session.GetInt32("User_Id");
    var holiday = await _db.Holidays
      .FirstOrDefaultAsync(h => h.Id == id);

    if (holiday == null || holiday.UserId != userId) return NotFound();
    
    var vm = new CreateHolidayViewModel
    {
        Id = holiday.Id,
        Title = holiday.Title,
        Location = holiday.Location,
        StartDate = holiday.StartDate,
        EndDate = holiday.EndDate,
        TotalCost = holiday.TotalCost,
        ThumbnailUrl = holiday.ThumbnailUrl,
    };

    return View("Edit", vm);
  }

  [HttpPost("/holidays/{id:int}/update", Name = "UpdateHolidayRoute")]
  public async Task<IActionResult> UpdateHoliday(CreateHolidayViewModel updatedHoliday, int id)
  {
    var holiday = await _db.Holidays
      .FirstOrDefaultAsync(h => h.Id == id);

    if (holiday == null) return NotFound();
    var userId = HttpContext.Session.GetInt32("User_Id");
    if (userId != holiday.UserId) return Forbid();

    if (updatedHoliday.StartDate > updatedHoliday.EndDate)
    {
      ModelState.AddModelError(nameof(updatedHoliday.EndDate),
      "End date cannot be before start date.");
    }

    if (!ModelState.IsValid)
    {
      return View("Details", updatedHoliday);
    }

    holiday.Title = updatedHoliday.Title;
    holiday.Location = updatedHoliday.Location;
    holiday.StartDate = updatedHoliday.StartDate;
    holiday.EndDate = updatedHoliday.EndDate;
    holiday.TotalCost = updatedHoliday.TotalCost;
    holiday.ThumbnailUrl = updatedHoliday.ThumbnailUrl;

    TempData["Success"] = "Holiday updated!";
    return RedirectToAction("GetHoliday", new { id = holiday.Id });
  }

  [HttpPost("/holidays/{id:int}/delete")]
  [ValidateAntiForgeryToken]
  public IActionResult Delete(int id)
  {
    var holiday = _db.Holidays.FirstOrDefault(h => h.Id == id);
    if (holiday == null) return NotFound();
    var userId = HttpContext.Session.GetInt32("User_Id");
    if (userId == null || userId != holiday?.UserId) return Redirect("/");
    _db.Holidays.Remove(holiday);
    _db.SaveChanges();

    return RedirectToAction("Index", "Dashboard");
  }

}