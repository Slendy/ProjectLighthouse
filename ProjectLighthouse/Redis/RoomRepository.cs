#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Types.Entities.Token;
using LBPUnion.ProjectLighthouse.Types.Redis;
using Redis.OM;
using Redis.OM.Searching;

namespace LBPUnion.ProjectLighthouse.Redis;

public class RoomRepository
{
    private readonly RedisCollection<RedisRoom> rooms;
    private readonly RedisConnectionProvider provider;

    public RoomRepository(RedisConnectionProvider provider)
    {
        this.provider = provider;
        this.rooms = (RedisCollection<RedisRoom>)provider.RedisCollection<RedisRoom>();
    }

    public async Task AddRoomAsync(RedisRoom room)
    {
        await this.rooms.InsertAsync(room);
        await this.ExtendRoomSessionAsync(room);
    }

    public async Task ExtendRoomSessionAsync(RedisRoom room)
    {
        await this.provider.Connection.ExecuteAsync("EXPIRE", $"Room:{room.Id}", "300");
    }

    public Task RemoveAsync(RedisRoom room) => this.rooms.DeleteAsync(room);

    public ValueTask SaveAsync() => this.rooms.SaveAsync();

    /// <summary>
    /// Finds the room that the given token is in 
    /// </summary>
    /// <param name="token">The token to search with</param>
    /// <returns>The room that the token is in</returns>
    public Task<RedisRoom?> GetRoomForToken(GameToken token) =>
        this.rooms.Where(r => r.RoomMembers.Contains(token.UserId))
            .Where(r => r.RoomPlatform == token.Platform && r.RoomVersion == token.GameVersion)
            .FirstOrDefaultAsync();

    public Task<IList<RedisRoom>> GetRoomsAsync() => this.rooms.ToListAsync();

    public Task<RedisRoom?> GetRoomAsync(Ulid roomId) => this.rooms.FindByIdAsync(roomId.ToString());

    public Task<RedisRoom?> GetRoomAsync(int hostId) => this.rooms.Where(r => r.RoomHostId == hostId).FirstOrDefaultAsync();

}