namespace VisitEmAll.Tests.Controllers;

public class DashboardControllerTests : NUnitTestBase
{
    private DashboardController _controller;

    [SetUp]
    public void LocalSetUp()
    {
        var logger = Mock.Of<ILogger<DashboardController>>();
        _controller = new DashboardController(_context, logger);

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

    [Test]
    public void Index_WhenNotLoggedIn_RedirectsToRoot()
    {
        var result = _controller.Index() as RedirectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Url, Is.EqualTo("/"));
    }

    [Test]
    public void Index_WhenLoggedIn_SetsViewData_ForCurrentUserAndHolidays()
    {
        var user = new User
        {
            Name = "Dash User",
            Email = $"dash{Guid.NewGuid()}@email.com",
            Password = "Password1!"
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        _controller.HttpContext.Session.SetInt32("User_Id", user.Id); 

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        _context.Holidays.AddRange(
            new Holiday { UserId = user.Id, Title = "Upcoming", StartDate = today.AddDays(10) },
            new Holiday { UserId = user.Id, Title = "Past", StartDate = today.AddDays(-10) }
        );
        _context.SaveChanges();

        var result = _controller.Index() as ViewResult;

        Assert.That(result, Is.Not.Null);

        var currentUser = _controller.ViewData["CurrentUser"] as User;
        Assert.That(currentUser, Is.Not.Null);
        Assert.That(currentUser!.Id, Is.EqualTo(user.Id));

        var upcoming = _controller.ViewData["UpcomingHolidays"] as List<Holiday>;
        var past = _controller.ViewData["PastHolidays"] as List<Holiday>;

        Assert.That(upcoming, Is.Not.Null);
        Assert.That(past, Is.Not.Null);

        Assert.That(upcoming!.Any(h => h.Title == "Upcoming"), Is.True);
        Assert.That(upcoming.Any(h => h.Title == "Past"), Is.False);

        Assert.That(past!.Any(h => h.Title == "Past"), Is.True);
        Assert.That(past.Any(h => h.Title == "Upcoming"), Is.False);
    }

    [Test]
    public void Index_OnlyIncludesCurrentUsersHolidays()
    {
        var u1 = new User { Name="U1", Email=$"u1{Guid.NewGuid()}@x.com", Password="Password1!" };
        var u2 = new User { Name="U2", Email=$"u2{Guid.NewGuid()}@x.com", Password="Password1!" };
        _context.Users.AddRange(u1, u2);
        _context.SaveChanges();

        _controller.HttpContext.Session.SetInt32("User_Id", u1.Id);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        _context.Holidays.AddRange(
            new Holiday { UserId = u1.Id, Title = "U1 Holiday", StartDate = today.AddDays(5) },
            new Holiday { UserId = u2.Id, Title = "U2 Holiday", StartDate = today.AddDays(5) }
        );
        _context.SaveChanges();

        _controller.Index();

        var upcoming = _controller.ViewData["UpcomingHolidays"] as List<Holiday>;
        Assert.That(upcoming, Is.Not.Null);
        Assert.That(upcoming!.Any(h => h.Title == "U1 Holiday"), Is.True);
        Assert.That(upcoming.Any(h => h.Title == "U2 Holiday"), Is.False);
    }
}