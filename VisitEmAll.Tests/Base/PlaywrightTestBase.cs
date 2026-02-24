namespace VisitEmAll.Tests.Base;


public class PlaywrightTestBase : PageTest
{
    protected const string BaseUrl = "http://127.0.0.1:5287";
    protected VisitEmAllDbContext _context;

    [SetUp]
    public async Task BaseSetUp()
    {
        var configuration = TestConfiguration.GetConfiguration();
        var connectionString = TestConfiguration.GetConnectionString();

        var options = new DbContextOptionsBuilder<VisitEmAllDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        _context = new VisitEmAllDbContext(options, configuration);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        DbSeeder.Seed(_context);

        _context.SaveChanges();
    }

    [TearDown]
    public void BaseTearDown()
    {
        _context?.Dispose();
    }
}