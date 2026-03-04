

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
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
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
    public async Task Create_Post_InvalidDatesReturnsViewWithError()
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
    public async Task Create_Post_ValidModelSavesToDbAndRedirects()
    {
        var country = new Country { Name = "Japan" };
        _context.Countries.Add(country);
        await _context.SaveChangesAsync();

        var uniqueTitle = $"Japan Trip {Guid.NewGuid()}";
        var vm = new CreateHolidayViewModel
        {
            Title = uniqueTitle,
            Location = "Tokyo",
            CountryId = country.Id,
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
    public async Task Create_Post_SingleDayTrip_CreatesExactlyOneDay()
    {
        var country = new Country { Name = "United Kingdom" };
        _context.Countries.Add(country);
        _context.SaveChanges();
        
        var vm = new CreateHolidayViewModel
        {
            Title = "Day Trip",
            CountryId = country.Id,
            StartDate = new DateOnly(2026, 7, 1),
            EndDate = new DateOnly(2026, 7, 1) // Same day
        };

        await _controller.Create(vm);

        var saved = _context.Holidays.Include(h => h.Days).First(h => h.Title == "Day Trip");
        Assert.That(saved.Days.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task EditHoliday_Get_ReturnsPreFilledForm()
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
    public async Task EditHoliday_Get_ReturnsNotFound_ForDifferentUser()
    {
        var someoneElse = new User { Name = "Other", Email = "other@test.com", Password = "Password1!" };
        _context.Users.Add(someoneElse);
        _context.SaveChanges();

        var othersHoliday = new Holiday { Title = "Private Trip", UserId = someoneElse.Id };
        _context.Holidays.Add(othersHoliday);
        _context.SaveChanges();

        var result = await _controller.EditHoliday(othersHoliday.Id);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task UpdateHoliday_Post_ValidChangesSavesAndRedirects()
    {
        var country = new Country { Name = "France" };
        _context.Countries.Add(country);
        _context.SaveChanges();

        var holiday = new Holiday
        {
            Title = "Old Title",
            UserId = _testUser.Id,
            CountryId = country.Id,
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 1, 2)
        };
        _context.Holidays.Add(holiday);
        _context.SaveChanges();

        var updatedVm = new CreateHolidayViewModel
        {
            Title = "New Title",
            Location = "New Location",
            CountryId = country.Id,
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 1, 3)
        };

        var result = await _controller.UpdateHoliday(updatedVm, holiday.Id) as RedirectResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Url, Is.EqualTo($"/holidays/{holiday.Id}"));

        var updatedHoliday = _context.Holidays.Find(holiday.Id);
        Assert.That(updatedHoliday.Title, Is.EqualTo("New Title"));
        Assert.That(updatedHoliday.EndDate, Is.EqualTo(new DateOnly(2026,1,3)));
    }

    [Test]
    public async Task UpdateHoliday_Post_ReturnsForbid_WhenUserMismatched()
    {
        var someoneElse = new User { Name = "Other", Email = "other2@test.com", Password = "Password1!" };
        _context.Users.Add(someoneElse);
        _context.SaveChanges();

        var othersHoliday = new Holiday { Title = "Secrets", UserId = someoneElse.Id };
        _context.Holidays.Add(othersHoliday);
        _context.SaveChanges();

        var updateVm = new CreateHolidayViewModel { Title = "Hacked Title" };

        var result = await _controller.UpdateHoliday(updateVm, othersHoliday.Id);

        Assert.That(result, Is.InstanceOf<ForbidResult>());
    }

    // [Test]
    // public void Delete_RemovesHolidayAndRedirects()
    // {
    //     var holiday = new Holiday
    //     {
    //         Title = "Delete Me",
    //         UserId = _testUser.Id
    //     };
    //     _context.Holidays.Add(holiday);
    //     _context.SaveChanges();

    //     var result = _controller.Delete(holiday.Id) as RedirectToActionResult;

    //     Assert.That(result.ActionName, Is.EqualTo("Index"));
    //     Assert.That(result.ControllerName, Is.EqualTo("Dashboard"));
    //     Assert.That(_context.Holidays.Any(h => h.Id == holiday.Id), Is.False);
    // }

    [Test]
    public async Task Details_Get_ReturnsViewWithHolidayAndDays()
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
    public async Task Details_ProperlyCategorizesDifferentItemTypes()
    {
        var holiday = new Holiday { Title = "Multi-Item Trip", UserId = _testUser.Id };
        var day = new HolidayDay { Date = new DateOnly(2026, 6, 1) };
        
        day.TimelineItems.Add(new DayActivity { Name = "Museum", Time = new TimeOnly(10, 0) });
        day.TimelineItems.Add(new DayRestaurant { Name = "Pasta Place", Time = new TimeOnly(19, 0) });
        
        holiday.Days.Add(day);
        _context.Holidays.Add(holiday);
        _context.SaveChanges();

        var result = await _controller.Details(holiday.Id) as ViewResult;
        var vm = result?.Model as HolidayDetailsViewModel;

        var items = vm.Days.First().Items;
        Assert.That(items.Any(i => i.ItemType == "Activity"), Is.True);
        Assert.That(items.Any(i => i.ItemType == "Restaurant"), Is.True);
    }
}