using VisitEmAll.Services;

namespace VisitEmAll.Tests.Controllers;

public class FriendsControllerTests : NUnitTestBase
{
    private FriendsController _controller;
    private FriendshipService _friendshipService;

    [SetUp]
    public void LocalSetUp()
    {
        _controller = new FriendsController(_context, _friendshipService);

        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockHttpSession();

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [TearDown]
    public void LocalTearDown()
    {
        _controller?.Dispose();
    }

}