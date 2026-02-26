
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
        
        var tempDataData = new TempDataDictionary(_controller.HttpContext, Mock.Of<ITempDataProvider>());
        _controller.TempData = tempDataData;

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

    [Test]
    public async Task Create_Post_InvalidDates_returnsViewWithError()
    {
        var vm = new CreateHolidayViewModel
        {
            Title = "Broken Trip",
            StartDate = new DateOnly(2026, 12, 31),
            EndDate = new DateOnly(2026, 01, 01) // Earlier
        };

        var result = await _controller.Create(vm) as ViewResult;

        Assert.That(_controller.ModelState.ContainsKey("EndDate"), Is.True);
        Assert.That(_controller.ModelState["EndDate"]?.Errors[0].ErrorMessage, 
                    Does.Contain("End date cannot be before start date."));
    }

    [Test]
    public async Task Create_Post_ValidModel_SavesToDbAndRedirects()
    {
        var vm = new CreateHolidayViewModel
        {
            Title = "Japan Trip",
            Location = "Tokyo",
            StartDate = new DateOnly(2026, 5, 1),
            EndDate = new DateOnly(2026, 5, 31),
            Activities = new List<CreateHolidayViewModel.ActivityInput>
            {
                new() { Name = "Pokemon Center" },
                new() { Name = "Mario Kart" }
            }
        };

        var result = await _controller.Create(vm) as RedirectToActionResult;

        Assert.That(result?.ActionName, Is.EqualTo("Index"));
        Assert.That(result?.ControllerName, Is.EqualTo("Home"));

        var savedHoliday = _context.Holidays.FirstOrDefault(h => h.Title == "Japan Trip");
        Assert.That(savedHoliday, Is.Not.Null);
        Assert.That(savedHoliday.UserId, Is.EqualTo(_testUser.Id));

        var savedActivities = _context.Activities.Where(a => a.HolidayId == savedHoliday.Id).ToList();
        Assert.That(savedActivities, Has.Count.EqualTo(2));
        Assert.That(savedActivities.Any(a => a.Name == "Pokemon Center"), Is.True);
    }
}