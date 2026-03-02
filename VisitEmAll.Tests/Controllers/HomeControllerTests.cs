
namespace VisitEmAll.Tests.Controllers;

public class HomeControllerTests : NUnitTestBase
{
    private HomeController _controller;

    [SetUp]
    public void LocalSetUp()
    {
        _controller = new HomeController();

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
    public void Index_FirstTimeLanding_ShowsLoginForm()
    {
        _controller.HttpContext.Session.Clear();

        var result = _controller.Index() as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo("Login"));
        Assert.That(result.ControllerName, Is.EqualTo("Auth"));
    }

    [Test]
    public void Privacy_ReturnsView()
    {
        var result = _controller.Privacy();

        Assert.That(result, Is.TypeOf<ViewResult>());
    }
}