using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VisitEmAll.Models;

namespace VisitEmAll.Controllers;

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        int? currentUserId = HttpContext.Session.GetInt32("user_id");
        if (currentUserId == null) return Redirect("/");
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}