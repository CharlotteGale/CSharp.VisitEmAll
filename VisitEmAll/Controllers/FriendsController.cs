using Microsoft.AspNetCore.Mvc;

using VisitEmAll.Services;
using VisitEmAll.Models;

namespace VisitEmAll.Controllers;

public class FriendsController : Controller
{
    private readonly VisitEmAllDbContext _context;
    private FriendshipService _friendshipService;

    public FriendsController(VisitEmAllDbContext context, FriendshipService friendshipService)
    {
        _context = context;
        _friendshipService = friendshipService;
    }

    [Route("/friends")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var currentUserId = HttpContext.Session.GetInt32("User_Id");
        if (!currentUserId.HasValue)
        {
            return RedirectToAction("Login", "Auth");
        }

        var friends = await _friendshipService.GetFriendsAsync(currentUserId.Value);
        return View(friends);
    }
}