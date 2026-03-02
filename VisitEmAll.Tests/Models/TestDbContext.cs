using Microsoft.EntityFrameworkCore;
using VisitEmAll.Models;

public class TestDbContext : VisitEmAllDbContext
{
    public TestDbContext(DbContextOptions<VisitEmAllDbContext> options, IConfiguration config)
        : base(options, config)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Prevent the real PostgreSQL provider from being registered
        // so tests can safely use InMemory
    }
}
