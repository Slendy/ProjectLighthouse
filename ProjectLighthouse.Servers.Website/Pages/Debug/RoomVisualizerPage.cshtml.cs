#nullable enable
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Redis;
using LBPUnion.ProjectLighthouse.Servers.Website.Pages.Layouts;
using LBPUnion.ProjectLighthouse.Types.Redis;
using Microsoft.AspNetCore.Mvc;
using Redis.OM;
#if !DEBUG
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
#endif

namespace LBPUnion.ProjectLighthouse.Servers.Website.Pages.Debug;

public class RoomVisualizerPage : BaseLayout
{

    public List<RedisRoom> Rooms = new();

    private readonly RoomRepository roomRepository;

    public RoomVisualizerPage(DatabaseContext database, RedisConnectionProvider redis) : base(database)
    {
        this.roomRepository = new RoomRepository(redis);
    }

    public async Task<IActionResult> OnGet()
    {
        #if !DEBUG
        User? user = this.Database.UserFromWebRequest(this.Request);
        if (user == null || !user.IsAdmin) return this.NotFound();
        #endif

        this.Rooms = (List<RedisRoom>)await this.roomRepository.GetRoomsAsync();

        return this.Page();
    }
}