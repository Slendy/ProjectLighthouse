using System.Threading.Tasks;

namespace LBPUnion.ProjectLighthouse.Types.Users;

public interface IFriendService
{
    public Task<UserFriendData> GetFriendsForUserAsync(int userId);

    public Task<UserFriendData> CreateFriendDataAsync(int userId);

    public Task UpdateFriendDataAsync(UserFriendData friendData);
}