using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Extensions;
using LBPUnion.ProjectLighthouse.Helpers;
using LBPUnion.ProjectLighthouse.Logging;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Entities.Token;
using LBPUnion.ProjectLighthouse.Types.Logging;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.MatchCommands;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using LBPUnion.ProjectLighthouse.Types.Users;

namespace LBPUnion.ProjectLighthouse.Servers.GameServer.Controllers.Matching;

[ApiController]
[Authorize]
[Route("LITTLEBIGPLANETPS3_XML/")]
[Produces("text/xml")]
public class MatchController : ControllerBase
{
    private readonly DatabaseContext database;

    public MatchController(DatabaseContext database)
    {
        this.database = database;
    }

    [HttpPost("gameState")]
    [Produces("text/plain")]
    public IActionResult GameState() => this.Ok("VALID");

    [HttpPost("match")]
    [Consumes("application/octet-stream")]
    [Produces("text/plain")]
    public async Task<IActionResult> MatchNew(IRoomService roomService)
    {
        GameTokenEntity token = this.GetToken();

        if (token.GameVersion is GameVersion.LittleBigPlanet1 or GameVersion.LittleBigPlanetPSP) return this.NotFound();

        UserEntity? user = await this.database.UserFromGameToken(token);
        if (user == null) return this.Forbid();

        string bodyString = await this.ReadBodyAsync();

        if (bodyString.Length == 0 || bodyString[0] != '[') return this.BadRequest();

        Logger.Debug("Received match data: " + bodyString, LogArea.Match);

        (string Type, string Data)? commandInfo = MatchHelper.ParseRoomCommand(bodyString);
        if (commandInfo == null) return this.BadRequest();

        string commandType = commandInfo.Value.Type;
        string commandData = commandInfo.Value.Data;

        RoomCommandData? roomData = null;
        try
        {
            roomData = JsonSerializer.Deserialize<RoomCommandData>(commandData);
        }
        catch (Exception e)
        {
            Logger.Error($"Exception while parsing matchData: roomData='{commandData}'", LogArea.Match);
            Logger.Error(e.ToDetailedException(), LogArea.Match);
        }

        if (roomData == null)
        {
            Logger.Error($"Could not parse match data: {nameof(roomData)} is null, roomData='{commandData}'",
                LogArea.Match);
            return this.BadRequest();
        }

        await LastContactHelper.SetLastContact(this.database, user, token.GameVersion, token.Platform);

        IMatchCommand command = commandType switch
        {
            "FindBestRoom" => new FindRoomCommand(token),
            "CreateRoom" => new UpdateRoomDataCommand(),
            "UpdateMyPlayerData" => new UpdateRoomDataCommand(),
            "UpdatePlayersInRoom" => new UpdatePlayersInRoomCommand(),
            _ => throw new ArgumentOutOfRangeException(nameof(roomService), @$"Invalid match command: {commandType}"),
        };
        Console.WriteLine(command.GetType());

        return await command.ProcessCommand(this.database, roomService, user, roomData);
    }

    [HttpPost("match2")]
    // [Consumes("application/octet-stream")]
    [Produces("text/plain")]
    public async Task<IActionResult> Match()
    {
        GameTokenEntity token = this.GetToken();

        UserEntity? user = await this.database.UserFromGameToken(token);
        if (user == null) return this.Forbid();

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
        catch (Exception e)
        {
            Logger.Error($"Exception while parsing matchData: body='{bodyString}'", LogArea.Match);
            Logger.Error(e.ToDetailedException(), LogArea.Match);

            return this.BadRequest();
        }

        if (matchData == null)
        {
            Logger.Error($"Could not parse match data: {nameof(matchData)} is null, body='{bodyString}'",
                LogArea.Match);
            return this.BadRequest();
        }

        Logger.Info($"Parsed match from {user.Username} (type: {matchData.GetType()})", LogArea.Match);

        #endregion

        await LastContactHelper.SetLastContact(this.database, user, token.GameVersion, token.Platform);

        #region Process match data

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
        //         FindBestRoomResponse? response = RoomHelper.FindBestRoom(this.database,
        //             user,
        //             token.GameVersion,
        //             diveInData.RoomSlot,
        //             token.Platform,
        //             token.UserLocation);
        //
        //         if (response == null) return this.NotFound();
        //
        //         string serialized = JsonSerializer.Serialize(response, typeof(FindBestRoomResponse));
        //         foreach (Player player in response.Players)
        //             MatchHelper.AddUserRecentlyDivedIn(user.UserId, player.User.UserId);
        //
        //         return this.Ok($"[{{\"StatusCode\":200}},{serialized}]");
        //     }
        //     case CreateRoom createRoom when !MatchHelper.UserLocations.IsEmpty:
        //     {
        //         List<int> users = new();
        //         foreach (string playerUsername in createRoom.Players)
        //         {
        //             UserEntity? player = await this.database.Users.FirstOrDefaultAsync(u => u.Username == playerUsername);
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
        //             List<UserEntity> users = new();
        //             foreach (string playerUsername in updatePlayersInRoom.Players)
        //             {
        //                 UserEntity? player = await this.database.Users.FirstOrDefaultAsync(u => u.Username == playerUsername);
        //                 // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        //                 if (player != null) users.Add(player);
        //                 else return this.BadRequest();
        //             }
        //
        //             room.PlayerIds = users.Select(u => u.UserId).ToList();
        //             await RoomHelper.CleanupRooms(this.database, null, room);
        //         }
        //         break;
        //     }
        // }

        #endregion

        return this.Ok("[{\"StatusCode\":200}]");
    }
}