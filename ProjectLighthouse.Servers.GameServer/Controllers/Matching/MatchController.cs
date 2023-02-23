#nullable enable
using System.Buffers.Binary;
using System.Net;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Extensions;
using LBPUnion.ProjectLighthouse.Helpers;
using LBPUnion.ProjectLighthouse.Logging;
using LBPUnion.ProjectLighthouse.Redis;
using LBPUnion.ProjectLighthouse.Types.Entities.Token;
using LBPUnion.ProjectLighthouse.Types.Logging;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.MatchCommands;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using LBPUnion.ProjectLighthouse.Types.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Redis.OM;

namespace LBPUnion.ProjectLighthouse.Servers.GameServer.Controllers.Matching;

[ApiController]
[Authorize]
[Route("LITTLEBIGPLANETPS3_XML/")]
[Produces("text/xml")]
public class MatchController : ControllerBase
{
    private readonly DatabaseContext database;
    private readonly RoomRepository rooms;
    private readonly UserRepository users;

    public MatchController(DatabaseContext database, RedisConnectionProvider redis)
    {
        this.database = database;
        this.rooms = new RoomRepository(redis);
        this.users = new UserRepository(redis);
    }

    [HttpPost("gameState")]
    [Produces("text/plain")]
    public IActionResult GameState() => this.Ok("VALID");

    [HttpPost("match")]
    [Produces("text/plain")]
    public async Task<IActionResult> Match()
    {
        GameToken token = this.GetToken();

        RedisUser user = await this.users.GetUser(token.UserId) ??
                         await this.users.CreateUser(token, await this.database.UsernameFromGameToken(token));

        await this.users.ExtendUserSession(user);

        RedisRoom? room = await this.rooms.GetRoomForToken(token);

        // If we can't find a room for the user, then create one
        // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
        if (room == null)
        {
            long location = BinaryPrimitives.ReadUInt32BigEndian(IPAddress.Parse(token.UserLocation).GetAddressBytes());
            Logger.Debug($"Creating room for {user.Username}, location={location}", LogArea.Match);
            room = new RedisRoom
            {
                RoomHostId = token.UserId,
                RoomMembers = new[]{token.UserId,},
                RoomBuildVersion = -1, // unknown
                RoomSlot = RoomSlot.PodSlot,
                RoomVersion = token.GameVersion,
                RoomState = RoomState.Unknown,
                RoomPlatform = token.Platform,
                MemberLocations = new Dictionary<int, long> {{token.UserId, location},},
            };
            await this.rooms.AddRoomAsync(room);
        }

        await this.rooms.ExtendRoomSessionAsync(room);

        #region Parse match data

        // Example POST /match: [UpdateMyPlayerData,["Player":"FireGamer9872"]]

        string bodyString = await this.ReadBodyAsync();

        if (bodyString.Length == 0 || bodyString[0] != '[') return this.BadRequest();

        Logger.Debug("Received match data: " + bodyString, LogArea.Match);

        IMatchCommand? matchData;
        try
        {
            matchData = MatchHelper.Deserialize(bodyString);
        }
        catch(Exception e)
        {
            Logger.Error("Exception while parsing matchData: ", LogArea.Match);
            Logger.Error(e.ToDetailedException(), LogArea.Match);

            return this.BadRequest();
        }

        if (matchData == null)
        {
            Logger.Error($"Could not parse match data: {nameof(matchData)} is null", LogArea.Match);
            return this.BadRequest();
        }

        Logger.Info($"Parsed match from {user.Username} (type: {matchData.GetType()})", LogArea.Match);

        #endregion

        await LastContactHelper.SetLastContact(this.database, token, token.GameVersion, token.Platform);

        string? response = await matchData.ProcessCommand(user, room, this.rooms, this.users);

        if (response == null) return this.BadRequest();

        return this.Ok(response);

        // #region Process match data
        //
        // switch (matchData)
        // {
        //     case UpdateMyPlayerData playerData:
        //     {
        //         MatchHelper.SetUserLocation(user.UserId, token.UserLocation);
        //         Room? room = RoomHelper.FindRoomByUser(user.UserId, token.GameVersion, token.Platform, true);
        //
        //         if (playerData.RoomState != null)
        //             if (room != null && Equals(room.HostId, user.UserId))
        //                 room.State = (RoomState)playerData.RoomState;
        //         break;
        //     }
        //     // Check how many people are online in release builds, disabled for debug for ..well debugging.
        //     #if DEBUG
        //     case FindBestRoom diveInData:
        //     #else
        //     case FindBestRoom diveInData when MatchHelper.UserLocations.Count > 1:
        //     #endif
        //     {
        //         FindBestRoomResponse? response = RoomHelper.FindBestRoom
        //             (user, token.GameVersion, diveInData.RoomSlot, token.Platform, token.UserLocation);
        //
        //         if (response == null) return this.NotFound();
        //
        //         string serialized = JsonSerializer.Serialize(response, typeof(FindBestRoomResponse));
        //         foreach (Player player in response.Players) MatchHelper.AddUserRecentlyDivedIn(user.UserId, player.User.UserId);
        //
        //         return this.Ok($"[{{\"StatusCode\":200}},{serialized}]");
        //     }
        //     case CreateRoom createRoom when MatchHelper.UserLocations.Count >= 1:
        //     {
        //         List<int> users = new();
        //         foreach (string playerUsername in createRoom.Players)
        //         {
        //             User? player = await this.database.Users.FirstOrDefaultAsync(u => u.Username == playerUsername);
        //             // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        //             if (player != null) users.Add(player.UserId);
        //             else return this.BadRequest();
        //         }
        //
        //         // Create a new one as requested
        //         RoomHelper.CreateRoom(users, token.GameVersion, token.Platform, createRoom.RoomSlot);
        //         break;
        //     }
        //     case UpdatePlayersInRoom updatePlayersInRoom:
        //     {
        //         Room? room = RoomHelper.Rooms.FirstOrDefault(r => r.HostId == user.UserId);
        //
        //         if (room != null)
        //         {
        //             List<User> users = new();
        //             foreach (string playerUsername in updatePlayersInRoom.Players)
        //             {
        //                 User? player = await this.database.Users.FirstOrDefaultAsync(u => u.Username == playerUsername);
        //                 // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        //                 if (player != null) users.Add(player);
        //                 else return this.BadRequest();
        //             }
        //
        //             room.PlayerIds = users.Select(u => u.UserId).ToList();
        //             await RoomHelper.CleanupRooms(null, room);
        //         }
        //
        //         break;
        //     }
        // }
        //
        // #endregion
    }
}