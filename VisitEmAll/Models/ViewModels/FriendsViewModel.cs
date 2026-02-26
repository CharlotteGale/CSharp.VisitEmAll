
namespace VisitEmAll.Models.ViewModels;

public class FriendsViewModel
{
    public List<User> AcceptedFriends { get; set; } = new();
    public List<Friendship> PendingRequests { get; set; } = new();

}