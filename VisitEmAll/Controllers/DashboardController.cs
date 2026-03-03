using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitEmAll.Models;

namespace VisitEmAll.Controllers;

public class DashboardController : Controller
{
    private readonly VisitEmAllDbContext _context;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(VisitEmAllDbContext context, ILogger<DashboardController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("/dashboard/{id?}")]
    public async Task<IActionResult> Index(int? id)
    {
        var currentUserId = HttpContext.Session.GetInt32("User_Id");
        if (currentUserId == null) return RedirectToAction("Login", "Auth");

        var targetUserId = id ?? currentUserId.Value;

        var user = await _context.Users
            .Include(u => u.Holidays)
            .FirstOrDefaultAsync(u => u.Id == targetUserId);

        ViewData["CurrentUserId"] = currentUserId;

        if (user == null) return NotFound();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var upcomingHolidays = user.Holidays
            .Where(h => h.StartDate >= today)
            .OrderBy(h => h.StartDate)
            .ToList();

        var pastHolidays = user.Holidays
            .Where(h => h.StartDate < today)
            .OrderByDescending(h => h.StartDate)
            .ToList();


        var likedHolidays = await _context.UserLikedHolidays
            .Where(x => x.UserId == currentUserId)
            .Include(x => x.Holiday)
                .ThenInclude(h => h.User)
            .Select(x => x.Holiday!)
            .ToListAsync();

        var likedHolidayIds = await _context.UserLikedHolidays 
        .Where(x => x.UserId == currentUserId) 
        .Select(x => x.HolidayId) 
        .ToListAsync();

        ViewData["CurrentUser"] = user; 
        ViewData["IsOwnDashboard"] = targetUserId == currentUserId;
        ViewData["UpcomingHolidays"] = upcomingHolidays;
        ViewData["PastHolidays"] = pastHolidays;
        ViewData["LikedHolidays"] = likedHolidays;
        ViewData["LikedHolidayIds"] = likedHolidayIds;



        return View(user);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}