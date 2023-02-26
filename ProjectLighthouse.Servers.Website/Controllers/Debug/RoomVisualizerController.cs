using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Helpers;
using LBPUnion.ProjectLighthouse.Redis;
using LBPUnion.ProjectLighthouse.Types.Redis;
using LBPUnion.ProjectLighthouse.Types.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Redis.OM;

namespace LBPUnion.ProjectLighthouse.Servers.Website.Controllers.Debug;

[ApiController]
[Route("debug/roomVisualizer")]
public class RoomVisualizerController : ControllerBase
{
    private readonly DatabaseContext database;
    private readonly RoomRepository roomRepository;

    public RoomVisualizerController(DatabaseContext database, RedisConnectionProvider redis)
    {
        this.database = database;
        this.roomRepository = new RoomRepository(redis);
    }

    [HttpGet("createFakeRoom")]
    public async Task<IActionResult> CreateFakeRoom()
    {
        #if !DEBUG
        await Task.FromResult(0);
        return this.NotFound();
        #else
        List<int> users = await this.database.Users.OrderByDescending(_ => EF.Functions.Random()).Take(2).Select(u => u.UserId).ToListAsync();
        this.roomRepository.CreateRoom()
        RoomHelperOld.CreateRoom(users, GameVersion.LittleBigPlanet2, Platform.PS3);

        foreach (int user in users)
        {
            MatchHelper.SetUserLocation(user, "127.0.0.1");
        }
        return this.Redirect("/debug/roomVisualizer");
        #endif
    }

    [HttpGet("deleteRooms")]
    public async Task<IActionResult> DeleteRooms()
    {
        #if !DEBUG
        return this.NotFound();
        #else
        foreach (RedisRoom room in await this.roomRepository.GetRoomsAsync())
        {
            await this.roomRepository.RemoveAsync(redisRoom);
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
        RoomHelperOld.CreateRoom(users, GameVersion.LittleBigPlanet2, Platform.PS3);
        RoomHelperOld.CreateRoom(users, GameVersion.LittleBigPlanet2, Platform.PS3);
        return this.Redirect("/debug/roomVisualizer");
        #endif
    }
}