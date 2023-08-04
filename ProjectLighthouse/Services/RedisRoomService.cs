using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace LBPUnion.ProjectLighthouse.Services;

public class RedisRoomService : IRoomService
{
    private readonly IRedisCollection<Room> rooms;

    public RedisRoomService(IRedisConnectionProvider provider)
    {
        this.rooms = provider.RedisCollection<Room>();
    }

    // Get room by id

    // Insert room

    // Delete room

    // Add player to existing room
}