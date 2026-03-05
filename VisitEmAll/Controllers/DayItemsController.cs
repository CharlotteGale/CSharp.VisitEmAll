using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitEmAll.Models;

namespace VisitEmAll.Controllers;

public class DayItemsController : Controller
{
    private readonly VisitEmAllDbContext _db;

    public DayItemsController(VisitEmAllDbContext db)
    {
        _db = db;
    }

    // -------------------------------------
    // CREATE ACTIVITY / RESTURANT / ACCOM
    // -------------------------------------
    [HttpPost("/day-items/{dayId:int}/activities/create")]
    public async Task<IActionResult> CreateActivity(int dayId, DayActivity vm)
    {
        var day = await _db.HolidayDays.FindAsync(dayId);
        if (day == null) return NotFound();

        vm.HolidayDayId = dayId;
        _db.DayItems.Add(vm);   
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", "Holidays", new { id = day.HolidayId });
    }

    [HttpPost("/day-items/{dayId:int}/restaurants/create")]
    public async Task<IActionResult> CreateRestaurant(int dayId, DayRestaurant vm)
    {
        var day = await _db.HolidayDays.FindAsync(dayId);
        if (day == null) return NotFound();

        vm.HolidayDayId = dayId;
        _db.DayItems.Add(vm);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", "Holidays", new { id = day.HolidayId });
    }

    [HttpPost("/day-items/{dayId:int}/accommodations/create")]
    public async Task<IActionResult> CreateAccommodation(int dayId, DayAccommodation vm)
    {
        var day = await _db.HolidayDays.FindAsync(dayId);
        if (day == null) return NotFound();

        vm.HolidayDayId = dayId;
        _db.DayItems.Add(vm);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", "Holidays", new { id = day.HolidayId });
    }
    // -------------------------------------
    // UPDATE ACTIVITY / RESTURANT / ACCOM 
    // -------------------------------------
    [HttpPost("/day-items/{id:int}/update")]
    public async Task<IActionResult> UpdateItem(
        int id,
        string name,
        TimeOnly? time,
        string? location,
        string? notes)
    {
        var item = await _db.DayItems.FirstOrDefaultAsync(i => i.Id == id);
        if (item == null) return NotFound();

        item.Name = name;
        item.Time = time;
        item.Location = location;
        item.Notes = notes;

        await _db.SaveChangesAsync();

        var day = await _db.HolidayDays.FindAsync(item.HolidayDayId);
        return RedirectToAction("Details", "Holidays", new { id = day!.HolidayId });
    }

    // -------------------------------------
    // DELETE ACTIVITY / RESTURANT / ACCOM
    // -------------------------------------

    [HttpPost("/day-items/{id:int}/delete")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var item = await _db.DayItems.FirstOrDefaultAsync(i => i.Id == id);
        if (item == null) return NotFound();

        var day = await _db.HolidayDays.FindAsync(item.HolidayDayId);

        _db.DayItems.Remove(item);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", "Holidays", new { id = day!.HolidayId });
    }

        [HttpPost("/day-items/{id:int}/edit-inline")]
        public async Task<IActionResult> EditInline(int id)
        {
            var item = await _db.DayItems.FirstOrDefaultAsync(i => i.Id == id);
            if (item == null) return NotFound();

            TempData["EditingItemId"] = id;

            var day = await _db.HolidayDays.FindAsync(item.HolidayDayId);
            return RedirectToAction("Details", "Holidays", new { id = day!.HolidayId });
        }

        [HttpPost("/day-items/{id:int}/cancel-inline")]
        public async Task<IActionResult> CancelInline(int id)
        {
            // Clear edit mode
            TempData["EditingItemId"] = null;

            var item = await _db.DayItems.FirstOrDefaultAsync(i => i.Id == id);
            if (item == null) return NotFound();

            var day = await _db.HolidayDays.FindAsync(item.HolidayDayId);
            return RedirectToAction("Details", "Holidays", new { id = day!.HolidayId });
        }

}