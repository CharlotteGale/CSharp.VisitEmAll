namespace VisitEmAll.Models;
public enum FriendshipStatus
{
    Pending = 0,
    Accepted = 1,
    Rejected = 2
}

public class Friendship
{
    public int Id { get; set; }

    public int RequesterId { get; set; }
    public User Requester { get; set; } = null!;

    public int ReceiverId { get; set; }
    public User Receiver { get; set; } = null!;

    public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

