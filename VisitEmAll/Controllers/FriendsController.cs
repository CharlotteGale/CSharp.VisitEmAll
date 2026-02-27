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

        var connectedUserIds = await _context.Friendships
                            .Where(f => f.RequesterId == currentUserId || f.ReceiverId == currentUserId)
                            .Select(f => f.RequesterId == currentUserId ? f.ReceiverId : f.RequesterId)
                            .ToListAsync();
        
        connectedUserIds.Add(currentUserId.Value);

        var potentialFriends = await _context.Users
                            .Where(u => !connectedUserIds.Contains(u.Id))
                            .ToListAsync();
        
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
                            .ToListAsync(),

            AllOtherUsers = potentialFriends
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

    [HttpPost]
    public async Task<IActionResult> Remove(int id)
    {
        var currentUserId = HttpContext.Session.GetInt32("User_Id");
        if(!currentUserId.HasValue)
        {
            return RedirectToAction("Login", "Auth");
        }

        await _friendshipService.RemoveFriendsAsync(currentUserId.Value, id);
        return RedirectToAction("Index");        
    }
}