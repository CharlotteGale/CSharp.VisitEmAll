

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

        _controller.HttpContext.Session.SetInt32("User_Id", _testUser.Id);
    }

    [TearDown]
    public void LocalTearDown()
    {
        _controller?.Dispose();
    }

    [Test]
    public void Create_Get_Returns_View_With_Initial_Activity()
    {
        var result = _controller.Create() as ViewResult;
        var model = result?.Model as CreateHolidayViewModel;

        Assert.That(result, Is.Not.Null);
        Assert.That(model.Activities, Has.Count.EqualTo(1));
    }

    [Test]
    public async Task Create_Post_Invalid_Dates_Returns_View_With_Error()
    {
        var vm = new CreateHolidayViewModel
        {
            Title = "Broken Trip",
            StartDate = new DateOnly(2026, 12, 31),
            EndDate = new DateOnly(2026, 01, 01) 
        };

        var result = await _controller.Create(vm) as ViewResult;

        Assert.That(_controller.ModelState.ContainsKey("EndDate"), Is.True);
        Assert.That(_controller.ModelState["EndDate"]?.Errors[0].ErrorMessage,
                    Does.Contain("End date cannot be before start date."));
    }

    [Test]
    public async Task Create_Post_Valid_Model_Saves_To_Db_And_Redirects()
    {
        var uniqueTitle = $"Japan Trip {System.Guid.NewGuid()}";
        var vm = new CreateHolidayViewModel
        {
            Title = uniqueTitle,
            Location = "Tokyo",
            StartDate = new DateOnly(2026, 5, 1),
            EndDate = new DateOnly(2026, 5, 3),
            Activities = new List<CreateHolidayViewModel.ActivityInput>
            {
                new() { Name = "Pokemon Center" },
                new() { Name = "Mario Kart" }
            }
        };

        var result = await _controller.Create(vm) as RedirectToActionResult;

        Assert.That(result?.ActionName, Is.EqualTo("Index"));
        Assert.That(result?.ControllerName, Is.EqualTo("Dashboard"));

        var savedHoliday = _context.Holidays.FirstOrDefault(h => h.Title == uniqueTitle);
        Assert.That(savedHoliday, Is.Not.Null);
        Assert.That(savedHoliday.UserId, Is.EqualTo(_testUser.Id));
        Assert.That(savedHoliday.Days.Count, Is.EqualTo(3)); 


    }

    [Test]
    public async Task Details_Returns_View_With_Holiday_And_Days()
    {
        var holiday = new Holiday
        {
            Title = "Test Holiday",
            UserId = _testUser.Id,
            StartDate = new DateOnly(2026, 5, 1),
            EndDate = new DateOnly(2026, 5, 3),
            Days = new List<HolidayDay>
            {
                new HolidayDay { Date = new DateOnly(2026,5,1) },
                new HolidayDay { Date = new DateOnly(2026,5,2) },
                new HolidayDay { Date = new DateOnly(2026,5,3) },
            }
        };
        _context.Holidays.Add(holiday);
        _context.SaveChanges();

        var result = await _controller.Details(holiday.Id) as ViewResult;
        var vm = result?.Model as HolidayDetailsViewModel;

        Assert.That(result, Is.Not.Null);
        Assert.That(vm.HolidayId, Is.EqualTo(holiday.Id));
        Assert.That(vm.Days.Count, Is.EqualTo(3));
    }


    [Test]
    public async Task Edit_Holiday_Get_Returns_Pre_Filled_Form()
    {
        var holiday = new Holiday
        {
            Title = "Edit Test",
            UserId = _testUser.Id,
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 1, 2)
        };
        _context.Holidays.Add(holiday);
        _context.SaveChanges();

        var result = await _controller.EditHoliday(holiday.Id) as ViewResult;
        var vm = result?.Model as CreateHolidayViewModel;

        Assert.That(result, Is.Not.Null);
        Assert.That(vm.Title, Is.EqualTo("Edit Test"));
        Assert.That(vm.StartDate, Is.EqualTo(new DateOnly(2026,1,1)));
    }

    [Test]
    public async Task Update_Holiday_Post_Valid_Changes_Saves_And_Redirects()
    {
        var holiday = new Holiday
        {
            Title = "Old Title",
            UserId = _testUser.Id,
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 1, 2)
        };
        _context.Holidays.Add(holiday);
        _context.SaveChanges();

        var updatedVm = new CreateHolidayViewModel
        {
            Title = "New Title",
            Location = "New Location",
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 1, 3)
        };

        var result = await _controller.UpdateHoliday(updatedVm, holiday.Id) as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo("GetHoliday"));

        var updatedHoliday = _context.Holidays.Find(holiday.Id);
        Assert.That(updatedHoliday.Title, Is.EqualTo("New Title"));
        Assert.That(updatedHoliday.EndDate, Is.EqualTo(new DateOnly(2026,1,3)));
    }


    [Test]
    public void Delete_Removes_Holiday_And_Redirects()
    {
        var holiday = new Holiday
        {
            Title = "Delete Me",
            UserId = _testUser.Id
        };
        _context.Holidays.Add(holiday);
        _context.SaveChanges();

        var result = _controller.Delete(holiday.Id) as RedirectToActionResult;

        Assert.That(result.ActionName, Is.EqualTo("Index"));
        Assert.That(result.ControllerName, Is.EqualTo("Dashboard"));
        Assert.That(_context.Holidays.Any(h => h.Id == holiday.Id), Is.False);
    }
}