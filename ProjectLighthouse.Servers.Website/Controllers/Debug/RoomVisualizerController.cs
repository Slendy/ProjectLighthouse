using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Helpers;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using LBPUnion.ProjectLighthouse.Types.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LBPUnion.ProjectLighthouse.Servers.Website.Controllers.Debug;

[ApiController]
[Route("debug/roomVisualizer")]
public class RoomVisualizerController : ControllerBase
{
    private readonly DatabaseContext database;
    private readonly IRoomService roomService;

    public RoomVisualizerController(DatabaseContext database, IRoomService roomService)
    {
        this.database = database;
        this.roomService = roomService;
    }

    [HttpGet("createFakeRoom")]
    public async Task<IActionResult> CreateFakeRoom()
    {
        #if !DEBUG
        await Task.FromResult(0);
        return this.NotFound();
        #else
        List<UserEntity> users = await this.database.Users.OrderByDescending(_ => EF.Functions.Random()).Take(2).ToListAsync();
        await this.roomService.InsertRoom(new NewRoom
        {
            RoomVersion = GameVersion.LittleBigPlanet2,
            RoomPlatform = Platform.PS3,
            Users = users.Select(u => u.Username).ToList(),
            RoomState = RoomState.Idle,
            RoomMood = RoomMood.AllowingAll,
            RoomSlot = RoomSlot.PodSlot,
        });

        return this.Redirect("/debug/roomVisualizer");
        #endif
    }

    [HttpGet("deleteRooms")]
    public async Task<IActionResult> DeleteRooms()
    {
        #if !DEBUG
        return this.NotFound();
        #else
        foreach (NewRoom room in await this.roomService.GetRooms())
        {
            await this.roomService.RemoveRoom(room);
        }

        return this.Redirect("/debug/roomVisualizer");
        #endif
    }

    [HttpGet("createRoomsWithDuplicatePlayers")]
    public async Task<IActionResult> CreateRoomsWithDuplicatePlayers()
    {
        #if !DEBUG
        await Task.FromResult(0);
        return this.NotFound();
        #else
        List<int> users = await this.database.Users.OrderByDescending(_ => EF.Functions.Random()).Take(1).Select(u => u.UserId).ToListAsync();
        RoomHelper.CreateRoom(users, GameVersion.LittleBigPlanet2, Platform.PS3);
        RoomHelper.CreateRoom(users, GameVersion.LittleBigPlanet2, Platform.PS3);
        return this.Redirect("/debug/roomVisualizer");
        #endif
    }
}