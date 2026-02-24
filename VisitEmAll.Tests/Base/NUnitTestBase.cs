namespace VisitEmAll.Tests.Base;

public class NUnitTestBase
{
    protected VisitEmAllDbContext _context;

    [SetUp]
    public void BaseSetUp()
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
    public void TearDown()
    {
        _context.Dispose();
    }
}