namespace VisitEmAll.Tests.Controllers;

public class DayItemsControllerTests : NUnitTestBase
{
    private DayItemsController _controller;
    private User _testUser;
    private Holiday _testHoliday;
    private HolidayDay _testDay;

    [SetUp]
    public void LocalSetUp()
    {
        _controller = new DayItemsController(_context);

        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockHttpSession();

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        _testUser = new User
        {
            Name = "Test User",
            Email = "test@example.com",
            Password = "Password1!"
        };
        _context.Users.Add(_testUser);
        _context.SaveChanges();

        _controller.HttpContext.Session.SetInt32("User_Id", _testUser.Id);

        _testHoliday = new Holiday
        {
            UserId = _testUser.Id,
            Title = "Test Holiday",
            Location = "Paris"
        };
        _context.Holidays.Add(_testHoliday);
        _context.SaveChanges();

        _testDay = new HolidayDay
        {
            HolidayId = _testHoliday.Id,
            Date = new DateOnly(2026, 6, 1)
        };
        _context.HolidayDays.Add(_testDay);
        _context.SaveChanges();

        var tempDataData = new TempDataDictionary(_controller.HttpContext, Mock.Of<ITempDataProvider>());
        _controller.TempData = tempDataData;
    }

    [Test]
    public async Task CreateActivity_ValidModel_SavesAndRedirectsToHolidayDetails()
    {
        var activity = new DayActivity { Name = "Eiffel Tower", Location = "Paris" };

        var result = await _controller.CreateActivity(_testDay.Id, activity) as RedirectToActionResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.ActionName, Is.EqualTo("Details"));
        Assert.That(result.RouteValues["id"], Is.EqualTo(_testHoliday.Id));

        var savedItem = await _context.DayItems.FirstOrDefaultAsync(i => i.Name == "Eiffel Tower");
        Assert.That(savedItem, Is.Not.Null);
        Assert.That(savedItem, Is.InstanceOf<DayActivity>());
    }

    [Test]
    public async Task CreateRestaurant_SavesCorrectType()
    {
        var restaurant = new DayRestaurant
        {
            Name = "Le Bistro",
            Location = "Paris"
        };

        var result = await _controller.CreateRestaurant(_testDay.Id, restaurant) as RedirectToActionResult;

        var saved = await _context.DayItems.FirstOrDefaultAsync(i => i.Name == "Le Bistro");

        Assert.That(saved, Is.Not.Null);
        Assert.That(saved, Is.TypeOf<DayRestaurant>());
        Assert.That(result.RouteValues["id"], Is.EqualTo(_testHoliday.Id));
    }

    [Test]
    public async Task CreateAccommodation_SavesCorrectType()
    {
        var hotel = new DayAccommodation
        {
            Name = "Grand Hotel",
            Location = "City Center"
        };

        await _controller.CreateAccommodation(_testDay.Id, hotel);

        var saved = await _context.DayItems.FirstOrDefaultAsync(i => i.Name == "Grand Hotel");

        Assert.That(saved, Is.Not.Null);
        Assert.That(saved, Is.TypeOf<DayAccommodation>());
    }

    [Test]
    public async Task UpdateItem_UpdatesFieldsCorrectly()
    {
        var originalItem = new DayActivity
        {
            HolidayDayId = _testDay.Id,
            Name = "Old Name",
            Location = "Old Location"
        };
        _context.DayItems.Add(originalItem);
        await _context.SaveChangesAsync();

        var newTime = new TimeOnly(14, 30);
        var newName = "New Name";

        var result = await _controller.UpdateItem(originalItem.Id, newName, newTime, "New Location", "New Notes") as RedirectToActionResult;

        var updated = await _context.DayItems.AsNoTracking().FirstOrDefaultAsync(i => i.Id == originalItem.Id);
        Assert.That(updated, Is.Not.Null);
        Assert.That(updated.Name, Is.EqualTo(newName));
        Assert.That(updated.Time, Is.EqualTo(newTime));
        Assert.That(result.ActionName, Is.EqualTo("Details"));
    }

    [Test]
    public async Task DeleteItem_RemovesFromDbAndRedirects()
    {
        var item = new DayActivity { HolidayDayId = _testDay.Id, Name = "To Be Deleted" };
        _context.DayItems.Add(item);
        await _context.SaveChangesAsync();

        var result = await _controller.DeleteItem(item.Id) as RedirectToActionResult;

        var exists = await _context.DayItems.AnyAsync(i => i.Id == item.Id);
        Assert.That(exists, Is.False);
        Assert.That(result.RouteValues["id"], Is.EqualTo(_testHoliday.Id));
    }

    [Test]
    public async Task EditInline_SetsTempData_AndRedirects()
    {
        var item = new DayActivity { HolidayDayId = _testDay.Id, Name = "Inline Item" };
        _context.DayItems.Add(item);
        await _context.SaveChangesAsync();

        var result = await _controller.EditInline(item.Id) as RedirectToActionResult;

        Assert.That(_controller.TempData["EditingItemId"], Is.EqualTo(item.Id));
        Assert.That(result.ActionName, Is.EqualTo("Details"));
        Assert.That(result.RouteValues["id"], Is.EqualTo(_testHoliday.Id));
    }

    [Test]
    public async Task CancelInline_ClearsTempData_AndRedirects()
    {
        var item = new DayActivity { HolidayDayId = _testDay.Id, Name = "Cancel Item" };
        _context.DayItems.Add(item);
        await _context.SaveChangesAsync();

        _controller.TempData["EditingItemId"] = item.Id;

        var result = await _controller.CancelInline(item.Id) as RedirectToActionResult;

        Assert.That(_controller.TempData["EditingItemId"], Is.Null);
        Assert.That(result.ActionName, Is.EqualTo("Details"));
    }

    [Test]
    public async Task EditInline_InvalidId_ReturnsNotFound()
    {
        var result = await _controller.EditInline(999);

        Assert.That(result, Is.TypeOf<NotFoundResult>());
    }
}