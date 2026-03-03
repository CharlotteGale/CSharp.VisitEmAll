

namespace VisitEmAll.Tests.Controllers;

public class FriendsControllerTests : NUnitTestBase
{
    private FriendsController _controller;
    private FriendshipService _friendshipService;

    private User _user;
    private User _friend;
    private User _pending;
    private User _stranger;

    [SetUp]
    public void LocalSetUp()
    {
        _friendshipService = new FriendshipService(_context);

        _controller = new FriendsController(_context, _friendshipService);

        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockHttpSession();

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        _user = new User { Name = "Test User", Email = "test@email.com", Password = "Password1!" };
        _friend = new User { Name = "Test Friend", Email = "friend@email.com", Password = "Password1!" };
        _pending = new User { Name = "Pending Friend", Email = "pending@email.com", Password = "Password1!" };
        _stranger = new User { Name = "Stranger", Email = "stranger@email.com", Password = "Password1!" };

        _context.Users.AddRange(_user, _friend, _pending, _stranger);
        _context.SaveChanges();

        _controller.HttpContext.Session.SetInt32("User_Id", _user.Id);

        _context.Friendships.AddRange(
            new Friendship { RequesterId = _user.Id, ReceiverId = _friend.Id, Status = FriendshipStatus.Accepted },
            new Friendship { RequesterId = _pending.Id, ReceiverId = _user.Id, Status = FriendshipStatus.Pending }
        );
        _context.SaveChanges();
    }

    [TearDown]
    public void LocalTearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        _controller?.Dispose();
    }

    [Test]
    public async Task Index_WhenNotLoggedIn_RedirectsToLogin()
    {
        _controller.HttpContext.Session.Remove("User_Id");

        var result = await _controller.Index() as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ActionName, Is.EqualTo("Login"));
    }

    [Test]
    public async Task Index_WhenLoggedIn_ReturnsViewWithCorrectViewModel()
    {
        var result = await _controller.Index() as ViewResult;
        var vm = result?.Model as FriendsViewModel;

        Assert.That(vm, Is.Not.Null);

        Assert.That(vm!.PendingRequests.Count, Is.EqualTo(1));
        Assert.That(vm.PendingRequests.First().RequesterId, Is.EqualTo(_pending.Id));

        Assert.That(vm.AllOtherUsers.Any(u => u.Id == _stranger.Id), Is.True);
        Assert.That(vm.AllOtherUsers.Any(u => u.Id == _friend.Id), Is.False);
    }

    [Test]
    public async Task Requests_WhenNotLoggedIn_RedirectsToLogin()
    {
        _controller.HttpContext.Session.Remove("User_Id");

        var result = await _controller.Requests() as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ActionName, Is.EqualTo("Login"));
    }

    [Test]
    public async Task Requests_ReturnsOnlyIncomingPending()
    {
        var result = await _controller.Requests() as ViewResult;
        var vm = result?.Model as FriendsViewModel;

        Assert.That(vm!.PendingRequests.Count, Is.EqualTo(1));
        Assert.That(vm.PendingRequests.First().RequesterId, Is.EqualTo(_pending.Id));
    }

    [Test]
    public async Task Accept_UpdatesStatusAndRedirects()
    {
        var friendship = _context.Friendships
                            .First(f => f.RequesterId == _pending.Id && f.ReceiverId == _user.Id);
        
        var result = await _controller.Accept(friendship.Id) as RedirectToActionResult;

        Assert.That(result!.ActionName, Is.EqualTo("Index"));

        var updatedFriendship = _context.Friendships.Find(friendship.Id);
        Assert.That(updatedFriendship!.Status, Is.EqualTo(FriendshipStatus.Accepted));
    }

    [Test]
    public async Task Reject_UpdatesStatusAndRedirects()
    {
        var friendship = _context.Friendships
                            .First(f => f.RequesterId == _pending.Id && f.ReceiverId == _user.Id);
        
        var result = await _controller.Reject(friendship.Id) as RedirectToActionResult;

        Assert.That(result!.ActionName, Is.EqualTo("Index"));

        var rejectedFriendship = _context.Friendships.Find(friendship.Id);
        Assert.That(rejectedFriendship, Is.Not.Null);
        Assert.That(rejectedFriendship!.Status, Is.EqualTo(FriendshipStatus.Rejected));
    }

    [Test]
    public async Task Remove_DeletesRelationshipAndRedirects()
    {
        int friendIdToRemove = _friend.Id;

        var result = await _controller.Remove(friendIdToRemove) as RedirectToActionResult;

        Assert.That(result!.ActionName, Is.EqualTo("Index"));

        var stillFriends = _context.Friendships.Any(f =>
                            (f.RequesterId == _user.Id && f.ReceiverId == friendIdToRemove) ||
                            (f.ReceiverId == _user.Id && f.RequesterId == friendIdToRemove));
        Assert.That(stillFriends, Is.False);
    }

    [Test]
    public async Task SendRequest_CreatesNewPendingFriendship()
    {
        int receiverId = _stranger.Id;

        var result = await _controller.SendRequest(receiverId) as RedirectToActionResult;
        Assert.That(result!.ActionName, Is.EqualTo("Index"));

        var friendship = _context.Friendships.FirstOrDefault(f =>
                            f.RequesterId == _user.Id && f.ReceiverId == receiverId);
        
        Assert.That(friendship, Is.Not.Null);
        Assert.That(friendship!.Status, Is. EqualTo(FriendshipStatus.Pending));
    }

    [Test]
    public async Task SendRequest_DoesNotCreateDuplicate_WhenAlreadyExists()
    {
        int receiverId = _friend.Id;
        var initialCount = _context.Friendships.Count();

        await _controller.SendRequest(receiverId);

        Assert.That(_context.Friendships.Count(), Is.EqualTo(initialCount));
    }
}