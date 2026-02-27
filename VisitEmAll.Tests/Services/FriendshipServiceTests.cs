using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using VisitEmAll.Models;
using VisitEmAll.Services;

namespace VisitEmAll.Tests.Services;

public class FriendshipServiceTests
{
    private VisitEmAllDbContext _context = null!;
    private FriendshipService _service = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<VisitEmAllDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var config = new ConfigurationBuilder().Build();

        _context = new TestDbContext(options, config);
        _service = new FriendshipService(_context);
    }

    private async Task SeedUsers()
    {
        _context.Users.AddRange(
            new User { Id = 1, Name = "Alice", Email = "a@test.com", Password = "x" },
            new User { Id = 2, Name = "Bob", Email = "b@test.com", Password = "x" },
            new User { Id = 3, Name = "Charlie", Email = "c@test.com", Password = "x" }
        );

        await _context.SaveChangesAsync();
    }


    [Test]
    public async Task CreateRequestAsync_Inserts_Pending_Row()
    {
        await SeedUsers();

        var result = await _service.CreateRequestAsync(1, 2);

        Assert.That(result.Status, Is.EqualTo(FriendshipStatus.Pending));
        Assert.That(_context.Friendships.Count(), Is.EqualTo(1));
    }


    [Test]
    public async Task AcceptAsync_Updates_Status_To_Accepted()
    {
        await SeedUsers();

        var request = await _service.CreateRequestAsync(1, 2);

        await _service.AcceptAsync(request.Id, 2);

        var updated = await _context.Friendships.FindAsync(request.Id);

        Assert.That(updated!.Status, Is.EqualTo(FriendshipStatus.Accepted));
    }


    [Test]
    public async Task RejectAsync_Updates_Status_To_Rejected()
    {
        await SeedUsers();

        var request = await _service.CreateRequestAsync(1, 2);

        await _service.RejectAsync(request.Id, 2);

        var updated = await _context.Friendships.FindAsync(request.Id);

        Assert.That(updated!.Status, Is.EqualTo(FriendshipStatus.Rejected));
    }


    [Test]
    public async Task GetFriendsAsync_Returns_Only_Accepted_Friends()
    {
        await SeedUsers();

        var accepted = await _service.CreateRequestAsync(1, 2);
        await _service.AcceptAsync(accepted.Id, 2);

        await _service.CreateRequestAsync(1, 3); 

        var friends = await _service.GetFriendsAsync(1);

        Assert.That(friends.Count, Is.EqualTo(1));
        Assert.That(friends.First().Id, Is.EqualTo(2));
    }
}