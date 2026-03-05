
namespace VisitEmAll.Tests.Controllers;

public class HomeControllerTests : NUnitTestBase
{
    private HomeController _controller;

    [SetUp]
    public void LocalSetUp()
    {
        _controller = new HomeController(_context);

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
    public async Task Index_FirstTimeLanding_ShowsLoginForm()
    {
        _controller.HttpContext.Session.Clear();

        var result = await _controller.Index() as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo("Login"));
        Assert.That(result.ControllerName, Is.EqualTo("Auth"));
    }

    [Test]
    public async Task Index_WhenLoggedIn_ReturnsViewModel()
    {
        _controller.HttpContext.Session.SetInt32("User_Id", 1);

        var result = await _controller.Index() as ViewResult;

        Assert.That(result, Is.Not.Null, "The action should return a ViewResult.");
    
        var model = result.Model as TravelStatsViewModel;
        Assert.That(model, Is.Not.Null, "The model should be of type TravelStatsViewModel.");
        Assert.That(model.TripsCount, Is.GreaterThanOrEqualTo(0));
        }

    [Test]
    public void Privacy_ReturnsView()
    {
        var result = _controller.Privacy();

        Assert.That(result, Is.TypeOf<ViewResult>());
    }
}