#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Logging;
using LBPUnion.ProjectLighthouse.Types.Entities.Token;
using LBPUnion.ProjectLighthouse.Types.Levels;
using LBPUnion.ProjectLighthouse.Types.Logging;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
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

    public async Task<RedisRoom> CreateRoom(GameToken token, string ipAddress)
    {
        RedisRoom room = new()
        {
            RoomHostId = token.UserId,
            RoomMembers = new[]
            {
                token.UserId,
            },
            RoomBuildVersion = -1, // unknown
            RoomSlot = RoomSlot.PodSlot,
            RoomVersion = token.GameVersion,
            RoomState = RoomState.Idle,
            RoomPlatform = token.Platform,
            MemberLocations = new Dictionary<int, string>
            {
                {
                    token.UserId, ipAddress
                },
            },
        };
        await this.CleanupRoomsForUser(token);
        await this.AddRoomAsync(room);
        return room;
    }

    public async Task CleanupRoomsForUser(GameToken token)
    {
        List<RedisRoom> userRooms = (List<RedisRoom>)await this.GetRoomsByToken(token).ToListAsync();
        foreach (RedisRoom r in userRooms)
        {
            // If they are the host of the room then delete the entire room
            if (r.RoomHostId == token.UserId) await this.RemoveAsync(r);

            // If they are only a member then delete them from the member list and remove their location
            r.RoomMembers = r.RoomMembers.Where(rm => rm != token.UserId).ToArray();
            r.MemberLocations.Remove(token.UserId);
            await this.rooms.UpdateAsync(r);
        }
    }

    public async Task AddRoomAsync(RedisRoom room)
    {
        await this.rooms.InsertAsync(room);
        await this.ExtendRoomSessionAsync(room);
    }

    public IRedisCollection<RedisRoom> FindMatchesForToken(GameToken token, RedisRoom room)
    {
        IRedisCollection<RedisRoom> roomQuery = this.rooms.Where(r => r.Id != room.Id)
            .Where(r => r.RoomState == RoomState.PlayingLevel || r.RoomState == RoomState.DivingInWaiting)
            .Where(r => r.RoomVersion == token.GameVersion && token.Platform == r.RoomPlatform)
            .Where(r => r.RoomBuildVersion == -1 || r.RoomBuildVersion == room.RoomBuildVersion);

        if (room.RoomSlot.SlotType != SlotType.Pod && room.RoomSlot.SlotId != 0)
        {
            roomQuery = this.rooms.Where(r => r.RoomSlot.SlotType == room.RoomSlot.SlotType)
                .Where(r => r.RoomSlot.SlotId == room.RoomSlot.SlotId);
        }

        return roomQuery;
    }

    public async Task ExtendRoomSessionAsync(RedisRoom room)
    {
        await this.provider.Connection.ExecuteAsync("EXPIRE", $"Room:{room.Id}", "300");
    }

    public Task RemoveAsync(RedisRoom room) => this.rooms.DeleteAsync(room);

    public ValueTask SaveAsync() => this.rooms.SaveAsync();

    private IRedisCollection<RedisRoom> GetRoomsByToken(GameToken token) =>
        this.rooms.Where(r => r.RoomMembers.Contains(token.UserId))
            .Where(r => (int)r.RoomPlatform == (int)token.Platform)
            .Where(r => (int)r.RoomVersion == (int)token.GameVersion);

    /// <summary>
    /// Finds the room that the given token is in 
    /// </summary>
    /// <param name="token">The token to search with</param>
    /// <returns>The room that the token is in</returns>
    public Task<RedisRoom?> GetRoomByToken(GameToken token) => this.GetRoomsByToken(token).FirstOrDefaultAsync();

    public Task<IList<RedisRoom>> GetRoomsAsync() => this.rooms.ToListAsync();

    public Task<RedisRoom?> GetRoomAsync(Ulid roomId) => this.rooms.FindByIdAsync(roomId.ToString());

    public Task<RedisRoom?> GetRoomAsync(int hostId) => this.rooms.Where(r => r.RoomHostId == hostId).FirstOrDefaultAsync();

}