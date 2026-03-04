using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitEmAll.Models;
using VisitEmAll.ViewModels;


namespace VisitEmAll.Controllers;

public class HolidaysController : Controller
{
    private readonly VisitEmAllDbContext _db;

        
    private readonly IWebHostEnvironment webHostEnvironment;

    public HolidaysController(VisitEmAllDbContext db, IWebHostEnvironment hostEnvironment)
    {
        _db = db;
        webHostEnvironment = hostEnvironment;

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

        // Date validation (main)
        if (vm.StartDate.HasValue && vm.EndDate.HasValue &&
            vm.EndDate.Value < vm.StartDate.Value)
        {
            ModelState.AddModelError(nameof(vm.EndDate),
                "End date cannot be before start date.");
        }

        // Country validation (your feature)
        if (vm.CountryId == null)
        {
            ModelState.AddModelError(nameof(vm.CountryId), "Please select a country.");
        }
        else
        {
            var exists = await _db.Countries.AnyAsync(c => c.Id == vm.CountryId.Value);
            if (!exists)
                ModelState.AddModelError(nameof(vm.CountryId), "Please select a valid country.");
        }

        if (!ModelState.IsValid)
        {
            vm.Activities ??= new();
            if (vm.Activities.Count == 0) vm.Activities.Add(new());
            return View(vm);
        }

        var userId = HttpContext.Session.GetInt32("User_Id");
        if (userId == null) return RedirectToAction("Login", "Auth");

        string uniqueFileName = null;
        if (vm.HeroImageFile != null)
        {
            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads/heros");
            
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            uniqueFileName = Guid.NewGuid().ToString() + "_" + vm.HeroImageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
            await vm.HeroImageFile.CopyToAsync(fileStream);
            }

            uniqueFileName = "/uploads/heros/" + uniqueFileName;
        }

        var holiday = new Holiday
        {
            UserId = userId.Value,
            Title = vm.Title,
            Location = vm.Location,
            StartDate = vm.StartDate,
            EndDate = vm.EndDate,
            TotalCost = vm.TotalCost,
            HeroImageUrl = uniqueFileName,
            Days = new List<HolidayDay>()
        };

        // Create HolidayDays if dates provided (main)
        if (vm.StartDate.HasValue && vm.EndDate.HasValue)
if (vm.StartDate.HasValue && vm.EndDate.HasValue) 
{ 
    for (var date = vm.StartDate.Value; date <= vm.EndDate.Value; date = date.AddDays(1)) 
    { holiday.Days.Add(new HolidayDay { Date = date }); 
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

        var holiday = await _db.Holidays
            .Include(h => h.Country)
            .Include(h => h.Days)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (holiday == null || holiday.UserId != userId)
            return NotFound();

        var vm = new CreateHolidayViewModel
        {
            Id = holiday.Id,
            Title = holiday.Title,
            Location = holiday.Location,
            StartDate = holiday.StartDate,
            EndDate = holiday.EndDate,
            TotalCost = holiday.TotalCost,
            ExistingHeroImage = holiday.HeroImageUrl,
            CountryId = holiday.CountryId,
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
        var holiday = await _db.Holidays
            .Include(h => h.Days)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (holiday == null)
            return NotFound();

        var userId = HttpContext.Session.GetInt32("User_Id");
        if (userId != holiday.UserId)
            return Forbid();

        // Date validation
        if (updatedHoliday.StartDate.HasValue && updatedHoliday.EndDate.HasValue &&
            updatedHoliday.EndDate.Value < updatedHoliday.StartDate.Value)
        {
            ModelState.AddModelError(nameof(updatedHoliday.EndDate),
                "End date cannot be before start date.");
        }

        // Country validation
        if (updatedHoliday.CountryId == null)
        {
            ModelState.AddModelError(nameof(updatedHoliday.CountryId), "Please select a country.");
        }
        else
        {
            var exists = await _db.Countries.AnyAsync(c => c.Id == updatedHoliday.CountryId.Value);
            if (!exists)
                ModelState.AddModelError(nameof(updatedHoliday.CountryId), "Please select a valid country.");
        }

        if (!ModelState.IsValid)
            return View("Edit", updatedHoliday);


        string uniqueFileName = null;

        if (updatedHoliday.HeroImageFile != null)
        {
            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads/heros");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            uniqueFileName = Guid.NewGuid().ToString() + "_" + updatedHoliday.HeroImageFile.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await updatedHoliday.HeroImageFile.CopyToAsync(fileStream);
            }
            
            uniqueFileName = "/uploads/heros/" + uniqueFileName;

            if (!string.IsNullOrEmpty(holiday.HeroImageUrl))
            {
                string oldFilePath = Path.Combine(uploadsFolder, holiday.HeroImageUrl);

                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }
            holiday.HeroImageUrl = uniqueFileName;
        }

        holiday.Title = updatedHoliday.Title;
        holiday.Location = updatedHoliday.Location;
        holiday.StartDate = updatedHoliday.StartDate;
        holiday.EndDate = updatedHoliday.EndDate;
        holiday.TotalCost = updatedHoliday.TotalCost;
        if (uniqueFileName != null)
            {
                holiday.HeroImageUrl = uniqueFileName;
            }
        holiday.CountryId = updatedHoliday.CountryId;

        await _db.SaveChangesAsync();

        await SyncHolidayDays(holiday);

        TempData["Success"] = "Holiday updated!";
        return Redirect($"/holidays/{holiday.Id}");
    }


    [HttpPost("/holidays/{id:int}/delete")]
    public async Task<IActionResult> DeleteHoliday(int id)
    {
        var holiday = await _db.Holidays.FirstOrDefaultAsync(i => i.Id == id);
        if (holiday == null) return NotFound();

        var day = await _db.HolidayDays.FindAsync(holiday.Id);

        _db.Holidays.Remove(holiday);
        await _db.SaveChangesAsync();

        return RedirectToAction("Index", "Dashboard");
    }
    
    // ---------------------------
    // SYNC HOLIDAY DAYS (HELPER)
    // ---------------------------

    private async Task SyncHolidayDays(Holiday holiday)
    {
        if (!holiday.StartDate.HasValue || !holiday.EndDate.HasValue)
            return;

        // DateOnly already contains only a date — no .Date needed
        var start = holiday.StartDate.Value;
        var end = holiday.EndDate.Value;

        var existingDays = holiday.Days.ToList();

        // Add missing days
        for (var date = start; date <= end; date = date.AddDays(1))
        {
            if (!existingDays.Any(d => d.Date == date))
            {
                _db.HolidayDays.Add(new HolidayDay
                {
                    HolidayId = holiday.Id,
                    Date = date
                });
            }
        }

        // Remove days outside the new range
        foreach (var day in existingDays)
        {
            if (day.Date < start || day.Date > end)
            {
                _db.HolidayDays.Remove(day);
            }
        }

        await _db.SaveChangesAsync();
    }


    // ---------------------------
    // DETAILS (READ-ONLY)
    // ---------------------------

[HttpGet("/holidays/{id:int}")]
    public async Task<IActionResult> Details(int id, string? addType = null, int? dayId = null)
    {
        var holiday = await _db.Holidays
            .Include(h => h.Country)
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
                .ToList(),

            AddType = addType,
            AddDayId = dayId
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
        [HttpPost("/holidays/{id:int}/like")]
        public async Task<IActionResult> Like(int id)
        {
            var userId = HttpContext.Session.GetInt32("User_Id");
            if (userId == null) return Unauthorized();

            var holiday = await _db.Holidays
                .Include(h => h.User)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (holiday == null) return NotFound();

            // Cannot like your own holiday
            if (holiday.UserId == userId) return BadRequest("You cannot like your own holiday.");

            bool alreadyLiked = await _db.UserLikedHolidays
                .AnyAsync(x => x.UserId == userId && x.HolidayId == id);

            if (!alreadyLiked)
            {
                _db.UserLikedHolidays.Add(new UserLikedHoliday
                {
                    UserId = userId.Value,
                    HolidayId = id
                });
                await _db.SaveChangesAsync();
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost("/holidays/{id:int}/unlike")]
        public async Task<IActionResult> Unlike(int id)
        {
            var userId = HttpContext.Session.GetInt32("User_Id");
            if (userId == null) return Unauthorized();

            var like = await _db.UserLikedHolidays
                .FirstOrDefaultAsync(x => x.UserId == userId && x.HolidayId == id);

            if (like != null)
            {
                _db.UserLikedHolidays.Remove(like);
                await _db.SaveChangesAsync();
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }


}
