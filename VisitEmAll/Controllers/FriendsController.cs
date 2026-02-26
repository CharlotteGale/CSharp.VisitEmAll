using Microsoft.AspNetCore.Mvc;

using VisitEmAll.Services;
using VisitEmAll.Models;

namespace VisitEmAll.Controllers;

public class FriendsController : Controller
{
    private readonly VisitEmAllDbContext _context;
    private FriendshipService _friendshipService;

    public FriendsController(VisitEmAllDbContext context)
    {
        _context = context;
    }

    [Route("/friends")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var currentUserId = HttpContext.Session.GetInt32("User_Id");
        if (currentUserId == null) return RedirectToAction("Login", "Auth");

        var friends = await _friendshipService.GetFriendsAsync(currentUserId.Value);
        return View(friends);
    }
}