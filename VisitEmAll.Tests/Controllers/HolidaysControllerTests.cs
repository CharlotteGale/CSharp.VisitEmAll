
namespace VisitEmAll.Tests.Controllers;

public class HolidaysControllerTests : NUnitTestBase
{
    private HolidaysController _controller;

    private User _testUser;

    [SetUp]
    public void LocalSetUp()
    {
        _controller = new HolidaysController(_context);

        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockHttpSession();

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        _testUser = new User
        {
            Name = "Test User",
            Email = "test@email.com",
            Password = "Password1!"
        };
        _context.Users.Add(_testUser);
        _context.SaveChanges();

        _controller.HttpContext.Session.SetInt32("user_id", _testUser.Id);
    }

    [TearDown]
    public void LocalTearDown()
    {
        _controller?.Dispose();
    }

    [Test]
    public void Create_Get_ReturnsViewWithInitialActivity()
    {
        var result = _controller.Create() as ViewResult;
        var model = result?.Model as CreateHolidayViewModel;

        Assert.That(result, Is.Not.Null);
        Assert.That(model.Activities, Has.Count.EqualTo(1));
    }
}