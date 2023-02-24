using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Extensions;
using LBPUnion.ProjectLighthouse.Helpers;
using LBPUnion.ProjectLighthouse.Redis;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Entities.Token;
using LBPUnion.ProjectLighthouse.Types.Redis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Redis.OM;

namespace LBPUnion.ProjectLighthouse.Servers.GameServer.Controllers;

[ApiController]
[Authorize]
[Route("LITTLEBIGPLANETPS3_XML/goodbye")]
[Produces("text/xml")]
public class LogoutController : ControllerBase
{

    private readonly DatabaseContext database;
    private readonly UserRepository userRepository;
    private readonly RoomRepository roomRepository;

    public LogoutController(DatabaseContext database, RedisConnectionProvider provider)
    {
        this.database = database;
        this.userRepository = new UserRepository(provider);
        this.roomRepository = new RoomRepository(provider);
    }

    [HttpPost]
    public async Task<IActionResult> OnPost()
    {
        GameToken token = this.GetToken();

        User? user = await this.database.UserFromGameToken(token);
        if (user == null) return this.StatusCode(403, "");

        user.LastLogout = TimeHelper.TimestampMillis;

        this.database.GameTokens.RemoveWhere(t => t.TokenId == token.TokenId);
        this.database.LastContacts.RemoveWhere(c => c.UserId == token.UserId);
        await this.database.SaveChangesAsync();


        RedisUser? redisUser = await this.userRepository.GetUser(token.UserId);
        if (redisUser == null) return this.Ok();

        RedisRoom? room = await this.roomRepository.GetRoomForToken(token);
        if (room != null)
        {
            // delete room if user was host
            if (room.RoomHostId == redisUser.UserId) await this.roomRepository.RemoveAsync(room);
            // remove room from user
            redisUser.Rooms = redisUser.Rooms.Where(rid => rid != room.Id).ToArray();
        }

        redisUser.GameTokens = redisUser.GameTokens.Where(id => id != token.TokenId).ToArray();
        if(redisUser.GameTokens.Length == 0)
            await this.userRepository.DeleteUser(redisUser);
        else
            await this.userRepository.SaveAsync();

        await this.roomRepository.SaveAsync();

        return this.Ok();
    }

    
}