using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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
    [Route("/dashboard")]
    [HttpGet]
    public IActionResult Index()
{
    int? currentUserId = HttpContext.Session.GetInt32("User_Id");
    if (currentUserId == null) return Redirect("/");

    var currentUser = _context.Users
        .FirstOrDefault(u => u.Id == currentUserId);

    var today = DateOnly.FromDateTime(DateTime.UtcNow);

    var userHolidays = _context.Holidays
        .Where(h => h.UserId == currentUserId)
        .ToList();

    var upcomingHolidays = userHolidays
    .Where(h => h.StartDate >= today)
    .OrderBy(h => h.StartDate)
    .ToList();

var pastHolidays = userHolidays
    .Where(h => h.StartDate < today)
    .OrderByDescending(h => h.StartDate)
    .ToList();

    ViewData["CurrentUser"] = currentUser;
    ViewData["UpcomingHolidays"] = upcomingHolidays;
    ViewData["PastHolidays"] = pastHolidays;

    return View();
}

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}