using Microsoft.AspNetCore.Mvc;

using VisitEmAll.Services;
using VisitEmAll.Models;
using Microsoft.EntityFrameworkCore;
using VisitEmAll.Models.ViewModels;

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

        // We pull everything here so the sidebar and the list are both full
        var vm = new FriendsViewModel
        {
            AcceptedFriends = await _friendshipService.GetFriendsAsync(currentUserId.Value),

            PendingRequests = await _context.Friendships
                            .Include(f => f.Requester)
                            .Where(f => f.ReceiverId == currentUserId.Value 
                                            && f.Status == FriendshipStatus.Pending)
                            .ToListAsync(),

            SentRequests = await _context.Friendships
                            .Include(f => f.Receiver)
                            .Where(f => f.RequesterId == currentUserId.Value 
                                            && f.Status == FriendshipStatus.Pending)
                            .ToListAsync()
        };

        return View(vm);
    }

    [HttpGet("requests")]
    public async Task<IActionResult> Requests()
    {
        var currentUserId = HttpContext.Session.GetInt32("User_Id");
        if (!currentUserId.HasValue)
        {
            return RedirectToAction("Login", "Auth");
        }

        var vm = new FriendsViewModel
        {
            AcceptedFriends = await _friendshipService.GetFriendsAsync(currentUserId.Value),

            PendingRequests = await _context.Friendships
                            .Include(f => f.Requester)
                            .Where(f => f.ReceiverId == currentUserId.Value 
                                            && f.Status == FriendshipStatus.Pending)
                            .ToListAsync(),

            SentRequests = await _context.Friendships
                            .Include(f => f.Receiver)
                            .Where(f => f.RequesterId == currentUserId.Value 
                                            && f.Status == FriendshipStatus.Pending)
                            .ToListAsync()
        };
        
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Accept(int id)
    {
        var currentUserId = HttpContext.Session.GetInt32("User_Id");
        if (!currentUserId.HasValue)
        {
            return RedirectToAction("Login", "Auth");
        }

        await _friendshipService.AcceptAsync(id, currentUserId.Value);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Reject(int id)
    {
        var currentUserId = HttpContext.Session.GetInt32("User_Id");
        if (!currentUserId.HasValue)
        {
            return RedirectToAction("Login", "Auth");
        }

        await _friendshipService.RejectAsync(id, currentUserId.Value);
        return RedirectToAction("Index");
    }
}