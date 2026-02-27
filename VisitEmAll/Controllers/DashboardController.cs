using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VisitEmAll.Models;
using VisitEmAll.ViewModels;

namespace VisitEmAll.Controllers;

public class DashboardController : Controller
{
    private readonly VisitEmAllDbContext _context;
    private readonly ILogger<Controller> _logger;

    public DashboardController(VisitEmAllDbContext context, ILogger<DashboardController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("/dashboard/{id?}")]
    public async Task<IActionResult> Index(int? id)
    {
        int? currentUserId = HttpContext.Session.GetInt32("User_Id");
        if (currentUserId == null) return RedirectToAction("Login", "Auth");

        int targetUserId = id ?? currentUserId.Value;

        var user = await _context.Users
            .Include(u => u.Holidays)
            .FirstOrDefaultAsync(u => u.Id == targetUserId);

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

        ViewData["CurrentUser"] = user; 
        ViewData["IsOwnDashboard"] = targetUserId == currentUserId;
        ViewData["UpcomingHolidays"] = upcomingHolidays;
        ViewData["PastHolidays"] = pastHolidays;

        return View(user);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}