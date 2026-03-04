namespace VisitEmAll.Tests.Controllers;

public class ComponentSandboxControllerTests : NUnitTestBase
{
    private ComponentSandboxController _controller;

    [SetUp]
    public void LocalSetUp()
    {
        var logger = Mock.Of<ILogger<ComponentSandboxController>>();
        _controller = new ComponentSandboxController();

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
    public void Bento_ReturnsViewWithPopulatedModel()
    {
        var result = _controller.Bento() as ViewResult;
        var vm = result?.Model as DashboardViewModel;

        Assert.That(result, Is.Not.Null);
        Assert.That(vm, Is.Not.Null);

        Assert.That(vm.TripsCount, Is.EqualTo(6));
        Assert.That(vm.MostVisitedCountryCode, Is.EqualTo("JP"));
    }
}