#nullable enable
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Extensions;
using LBPUnion.ProjectLighthouse.Servers.GameServer.Types.Users;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Entities.Token;
using LBPUnion.ProjectLighthouse.Types.Serialization;
using LBPUnion.ProjectLighthouse.Types.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace LBPUnion.ProjectLighthouse.Servers.GameServer.Controllers;

[ApiController]
[Authorize]
[Route("LITTLEBIGPLANETPS3_XML/")]
[Produces("text/xml")]
public class FriendsController : ControllerBase
{
    private readonly DatabaseContext database;
    private readonly RedisCollection<UserFriendData> friendStore;

    public FriendsController(DatabaseContext database, IRedisConnectionProvider provider)
    {
        this.database = database;
        this.friendStore = (RedisCollection<UserFriendData>)provider.RedisCollection<UserFriendData>();
    }

    [HttpPost("npdata")]
    public async Task<IActionResult> NPData()
    {
        GameTokenEntity token = this.GetToken();

        NPData? npData = await this.DeserializeBody<NPData>();
        if (npData == null) return this.BadRequest();

        List<UserEntity> friends = new();
        foreach (string friendName in npData.Friends ?? new List<string>())
        {
            UserEntity? friend = await this.database.Users.FirstOrDefaultAsync(u => u.Username == friendName);
            if (friend == null) continue;

            friends.Add(friend);
        }

        List<int> blockedUsers = new();
        foreach (string blockedUserName in npData.BlockedUsers ?? new List<string>())
        {
            UserEntity? blockedUser = await this.database.Users.FirstOrDefaultAsync(u => u.Username == blockedUserName);
            if (blockedUser == null) continue;

            blockedUsers.Add(blockedUser.UserId);
        }

        UserFriendData? friendData = await this.friendStore.FirstOrDefaultAsync(f => f.UserId == token.UserId);
        friendData ??= new UserFriendData
        {
            UserId = token.UserId,
        };

        friendData.FriendIds = friends.Select(u => u.UserId).ToList();
        friendData.BlockedIds = blockedUsers;

        await this.friendStore.UpdateAsync(friendData);

        List<MinimalUserProfile> minimalFriends =
            friends.Select(u => new MinimalUserProfile
            {
                UserHandle = new NpHandle(u.Username, ""),
            }).ToList();

        return this.Ok(new FriendResponse(minimalFriends));
    }

    [HttpGet("myFriends")]
    public async Task<IActionResult> MyFriends()
    {
        GameTokenEntity token = this.GetToken();

        UserFriendData? friendData = await this.friendStore.FirstOrDefaultAsync(f => f.UserId == token.UserId);

        GenericUserResponse<GameUser> response = new("myFriends", new List<GameUser>());

        if (friendData == null)
            return this.Ok(response);

        foreach (int friendId in friendData.FriendIds)
        {
            UserEntity? friend = await this.database.Users.FirstOrDefaultAsync(u => u.UserId == friendId);
            if (friend == null) continue;

            response.Users.Add(GameUser.CreateFromEntity(friend, token.GameVersion));
        }

        return this.Ok(response);
    }
}