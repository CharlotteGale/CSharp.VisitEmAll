using Microsoft.EntityFrameworkCore;
using VisitEmAll.Models;

namespace VisitEmAll.Services;

public class FriendshipService
{
    private readonly VisitEmAllDbContext _db;

    public FriendshipService(VisitEmAllDbContext db) => _db = db;

    public async Task<Friendship> CreateRequestAsync(int requesterId, int receiverId)
    {
        if (requesterId == receiverId)
            throw new InvalidOperationException("A user cannot friend themselves.");

        // Block duplicates in either direction (pending/accepted)
        var existing = await _db.Friendships.AnyAsync(f =>
            (f.RequesterId == requesterId && f.ReceiverId == receiverId) ||
            (f.RequesterId == receiverId && f.ReceiverId == requesterId));

        if (existing)
            throw new InvalidOperationException("Duplicate friend request prevented.");

        var friendship = new Friendship
        {
            RequesterId = requesterId,
            ReceiverId = receiverId,
            Status = FriendshipStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _db.Friendships.Add(friendship);
        await _db.SaveChangesAsync();
        return friendship;
    }

    public async Task AcceptAsync(int friendshipId, int actingUserId)
    {
        var friendship = await _db.Friendships.FirstAsync(f => f.Id == friendshipId);

        if (friendship.ReceiverId != actingUserId)
            throw new InvalidOperationException("Only the receiver can accept.");

        friendship.Status = FriendshipStatus.Accepted;
        await _db.SaveChangesAsync();
    }

    public async Task RejectAsync(int friendshipId, int actingUserId)
    {
        var friendship = await _db.Friendships.FirstAsync(f => f.Id == friendshipId);

        if (friendship.ReceiverId != actingUserId)
            throw new InvalidOperationException("Only the receiver can reject.");

        friendship.Status = FriendshipStatus.Rejected;
        await _db.SaveChangesAsync();
    }

    public async Task<List<User>> GetFriendsAsync(int userId)
    {
        // Only Accepted are "active"
        var accepted = _db.Friendships
            .Where(f => f.Status == FriendshipStatus.Accepted &&
                        (f.RequesterId == userId || f.ReceiverId == userId))
            .Select(f => f.RequesterId == userId ? f.Receiver : f.Requester);

        return await accepted.Distinct().ToListAsync();
    }

    public async Task RemoveFriendsAsync(int userId, int friendId)
    {
        var friendship = await _db.Friendships
            .FirstOrDefaultAsync(f =>
                (f.RequesterId == userId && f.ReceiverId == friendId ||
                f.RequesterId == friendId && f.ReceiverId == userId)
                && f.Status == FriendshipStatus.Accepted);
        
        if(friendship != null)
        {
            _db.Friendships.Remove(friendship);
            await _db.SaveChangesAsync();
        }
    }

    public async Task<int> GetPendingRequestCountAsync(int userId)
    {
        return await _db.Friendships
            .CountAsync(f => f.ReceiverId == userId && f.Status == FriendshipStatus.Pending);
    }
}