using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Types.Users;
using Redis.OM.Searching;

namespace LBPUnion.ProjectLighthouse.Services;

public class RedisFriendService : IFriendService
{
    private readonly IRedisCollection<UserFriendData> friends;

    public RedisFriendService(IRedisCollection<UserFriendData> friends)
    {
        this.friends = friends;
    }

    #nullable enable
    public Task<UserFriendData?> GetFriendsForUserAsync(int userId)
    {
        return this.friends.FirstOrDefaultAsync(u => u.UserId == userId);
    }
    #nullable restore

    public async Task<UserFriendData> CreateFriendDataAsync(int userId)
    {
        UserFriendData friendData = new()
        {
            UserId = userId,
        };

        await this.friends.InsertAsync(friendData);
        return friendData;
    }

    public Task UpdateFriendDataAsync(UserFriendData friendData) => this.friends.UpdateAsync(friendData);
}