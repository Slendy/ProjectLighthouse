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

namespace LBPUnion.ProjectLighthouse.Servers.GameServer.Controllers;

[ApiController]
[Authorize]
[Route("LITTLEBIGPLANETPS3_XML/")]
[Produces("text/xml")]
public class FriendsController : ControllerBase
{
    private readonly DatabaseContext database;
    private readonly IFriendService friendService;

    public FriendsController(DatabaseContext database, IFriendService friendService)
    {
        this.database = database;
        this.friendService = friendService;
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
            int blockedUserId = await this.database.Users.Where(u => u.Username == blockedUserName)
                .Select(u => u.UserId)
                .FirstOrDefaultAsync();
            if (blockedUserId == 0) continue;

            blockedUsers.Add(blockedUserId);
        }

        UserFriendData friendStore = await this.friendService.GetFriendsForUserAsync(token.UserId) ??
                                     await this.friendService.CreateFriendDataAsync(token.UserId);

        friendStore.FriendIds = friends.Select(u => u.UserId).ToList();
        friendStore.BlockedIds = blockedUsers;

        await this.friendService.UpdateFriendDataAsync(friendStore);

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

        UserFriendData? friendStore = await this.friendService.GetFriendsForUserAsync(token.UserId);

        GenericUserResponse<GameUser> response = new("myFriends", new List<GameUser>());

        if (friendStore == null)
            return this.Ok(response);

        foreach (int friendId in friendStore.FriendIds)
        {
            UserEntity? friend = await this.database.Users.FirstOrDefaultAsync(u => u.UserId == friendId);
            if (friend == null) continue;

            response.Users.Add(GameUser.CreateFromEntity(friend, token.GameVersion));
        }

        return this.Ok(response);
    }
}