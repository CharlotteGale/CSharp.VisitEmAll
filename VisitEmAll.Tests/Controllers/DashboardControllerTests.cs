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
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        _controller?.Dispose();
    }

    [Test]
    public async Task Index_WhenNotLoggedIn_RedirectsToRoot()
    {
        _controller.HttpContext.Session.Remove("User_Id");

        var result = await _controller.Index(null) as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ActionName, Is.EqualTo("Login"));
    }

    [Test]
    public async Task Index_WhenLoggedIn_SetsViewData_ForCurrentUserAndHolidays()
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

        var result = await _controller.Index(null) as ViewResult;

        Assert.That(result, Is.Not.Null);

        var vm = result.Model as DashboardViewModel;
        Assert.That(vm, Is.Not.Null, "Controller should return a DashboardViewModel");

        Assert.That(vm.User, Is.Not.Null);
        Assert.That(vm.User.Id, Is.EqualTo(user.Id));

        Assert.That(vm.UpcomingHolidays, Is.Not.Null);
        Assert.That(vm.PastHolidays, Is.Not.Null);

        Assert.That(vm.UpcomingHolidays.Any(h => h.Title == "Upcoming"), Is.True);
        Assert.That(vm.UpcomingHolidays.Any(h => h.Title == "Past"), Is.False);

        Assert.That(vm.PastHolidays.Any(h => h.Title == "Past"), Is.True);
        Assert.That(vm.PastHolidays.Any(h => h.Title == "Upcoming"), Is.False);
    }

    [Test]
    public async Task Index_OnlyIncludesCurrentUsersHolidays()
    {
        var u1 = new User { Name = "U1", Email = $"u1{Guid.NewGuid()}@x.com", Password = "Password1!" };
        var u2 = new User { Name = "U2", Email = $"u2{Guid.NewGuid()}@x.com", Password = "Password1!" };
        _context.Users.AddRange(u1, u2);
        _context.SaveChanges();

        _controller.HttpContext.Session.SetInt32("User_Id", u1.Id);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        _context.Holidays.AddRange(
            new Holiday { UserId = u1.Id, Title = "U1 Holiday", StartDate = today.AddDays(5) },
            new Holiday { UserId = u2.Id, Title = "U2 Holiday", StartDate = today.AddDays(5) }
        );
        _context.SaveChanges();

        var result = await _controller.Index(null) as ViewResult;
        var vm = result?.Model as DashboardViewModel;

        Assert.That(vm, Is.Not.Null, "The model should be a DashboardViewModel");

        Assert.That(vm.UpcomingHolidays, Is.Not.Null);
        Assert.That(vm.UpcomingHolidays.Any(h => h.Title == "U1 Holiday"), Is.True, "Should include User 1's holiday");
        Assert.That(vm.UpcomingHolidays.Any(h => h.Title == "U2 Holiday"), Is.False, "Should NOT include User 2's holiday");
    }
}