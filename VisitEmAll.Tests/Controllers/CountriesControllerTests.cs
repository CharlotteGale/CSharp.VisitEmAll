namespace VisitEmAll.Tests.Controllers;

public class CountriesControllerTests : NUnitTestBase
{
    private CountriesController _controller;

    [SetUp]
    public void LocalSetUp()
    {
        _controller = new CountriesController(_context);
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new MockHttpSession();

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        _context.Countries.RemoveRange(_context.Countries);
        _context.SaveChanges();
    }

    [TearDown]
    public void LocalTearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        _controller?.Dispose();
    }

    [Test]
    public async Task Search_TooShortQuery_ReturnsEmptyList()
    {
        var result = await _controller.Search(" ") as JsonResult;

        Assert.That(result.Value, Is.InstanceOf<Array>());

        var list = result.Value as IEnumerable<object>;
        Assert.That(list, Is.Empty);
    }

    [Test]
    public async Task Search_ValidQuery_ReturnsMatchingCountries()
    {
        _context.Countries.AddRange(new List<Country>
        {
            new() { Name = "Japan", Iso2 = "JP", Continent = "Asia" },
            new() { Name = "Jamaica", Iso2 = "JM", Continent = "Americas" },
            new() { Name = "Canada", Iso2 = "CA", Continent = "Americas" }
        });
        _context.SaveChanges();

        var result = await _controller.Search("ja") as JsonResult;

        var list = result.Value as System.Collections.IEnumerable;
        var listAsObj = list.Cast<object>().ToList();
        Assert.That(listAsObj, Has.Count.EqualTo(2));
    }
}