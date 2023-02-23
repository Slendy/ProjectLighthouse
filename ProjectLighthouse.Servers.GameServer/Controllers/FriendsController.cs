#nullable enable
using System.Text;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Extensions;
using LBPUnion.ProjectLighthouse.Helpers;
using LBPUnion.ProjectLighthouse.Redis;
using LBPUnion.ProjectLighthouse.Serialization;
using LBPUnion.ProjectLighthouse.Servers.GameServer.Types.Users;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Entities.Token;
using LBPUnion.ProjectLighthouse.Types.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Redis.OM;

namespace LBPUnion.ProjectLighthouse.Servers.GameServer.Controllers;

[ApiController]
[Authorize]
[Route("LITTLEBIGPLANETPS3_XML/")]
public class FriendsController : ControllerBase
{
    private readonly DatabaseContext database;
    private readonly UserRepository userRepository;

    public FriendsController(DatabaseContext database, RedisConnectionProvider redis)
    {
        this.database = database;
        this.userRepository = new UserRepository(redis);
    }

    [HttpPost("npdata")]
    public async Task<IActionResult> NPData()
    {
        GameToken token = this.GetToken();

        RedisUser? user = await this.userRepository.GetUser(token.UserId);

        if (user == null) return this.BadRequest();

        NPData? npData = await this.DeserializeBody<NPData>();
        if (npData == null) return this.BadRequest();

        SanitizationHelper.SanitizeStringsInClass(npData);

        List<string> friends = new();
        foreach (string friendName in npData.Friends ?? new List<string>())
        {
            string? friend = await this.database.Users.Where(u => u.Username == friendName)
                .Select(u => u.Username)
                .FirstOrDefaultAsync();
            if (friend == null) continue;

            friends.Add(friend);
        }

        List<string> blockedUsers = new();
        foreach (string blockedUserName in npData.BlockedUsers ?? new List<string>())
        {
            string? blockedUser = await this.database.Users.Where(u => u.Username == blockedUserName)
                .Select(u => u.Username)
                .FirstOrDefaultAsync();
            if (blockedUser == null) continue;

            blockedUsers.Add(blockedUser);
        }

        user.BlockedList = friends.ToArray();
        user.FriendList = blockedUsers.ToArray();
        await this.userRepository.UpdateUser(user);

        string friendsSerialized = friends.Aggregate(string.Empty, (current, username) => current + LbpSerializer.StringElement("npHandle", username));

        return this.Ok(LbpSerializer.StringElement("npdata", LbpSerializer.StringElement("friends", friendsSerialized)));
    }

    [HttpGet("myFriends")]
    public async Task<IActionResult> MyFriends()
    {
        GameToken token = this.GetToken();

        RedisUser? user = await this.userRepository.GetUser(token.UserId);

        if (user == null)
            return this.Ok(LbpSerializer.BlankElement("myFriends"));

        StringBuilder friends = new();
        foreach (string friendUsername in user.FriendList)
        {
            User? friendUser = await this.database.Users.FirstOrDefaultAsync(u => u.Username == friendUsername);
            if (friendUser == null) continue;

            friends.Append(friendUser.Serialize(token.GameVersion));
        }

        return this.Ok(LbpSerializer.StringElement("myFriends", friends));
    }
}