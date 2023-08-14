using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Extensions;
using LBPUnion.ProjectLighthouse.Servers.GameServer.Types.Users;
using LBPUnion.ProjectLighthouse.Types.Entities.Friends;
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
    private readonly MemoryContext memoryStore;

    public FriendsController(DatabaseContext database, MemoryContext memoryStore)
    {
        this.database = database;
        this.memoryStore = memoryStore;
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

        foreach (UserEntity friend in friends)
        {
            FriendEntity? friendEntity = await this.memoryStore.Friends.FirstOrDefaultAsync(f => f.FriendId == friend.UserId);
            if (friendEntity != null) continue;

            friendEntity = new FriendEntity
            {
                UserId = token.UserId,
                FriendId = friend.UserId,
            };
            this.memoryStore.Friends.Add(friendEntity);
        }

        await this.memoryStore.SaveChangesAsync();

        // friendStore.FriendIds = friends.Select(u => u.UserId).ToList();
        // friendStore.BlockedIds = blockedUsers;
        //
        // await this.friendService.UpdateFriendDataAsync(friendStore);

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

        List<int> friends = await this.memoryStore.Friends.Where(f => f.UserId == token.UserId)
            .Select(f => f.FriendId)
            .ToListAsync();

        GenericUserResponse<GameUser> response = new("myFriends", new List<GameUser>());

        if (friends.Count == 0)
            return this.Ok(response);

        foreach (int friendId in friends)
        {
            UserEntity? friend = await this.database.Users.FirstOrDefaultAsync(u => u.UserId == friendId);
            if (friend == null) continue;

            response.Users.Add(GameUser.CreateFromEntity(friend, token.GameVersion));
        }

        return this.Ok(response);
    }
}