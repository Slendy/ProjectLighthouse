#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using Redis.OM;
using Redis.OM.Searching;

namespace LBPUnion.ProjectLighthouse.Services;

public class RedisRoomService : IRoomService
{
    private readonly IRedisCollection<NewRoom> rooms;

    public RedisRoomService(IRedisCollection<NewRoom> rooms)
    {
        this.rooms = rooms;
    }

    public ValueTask<int> GetPlayerCountAsync() => this.rooms.SumAsync(r => r.Users.Count);

    public Task<int> GetRoomCountAsync() => this.rooms.CountAsync();

    public Task<NewRoom?> GetRoomForUser(int userId) =>
        this.rooms.FirstOrDefaultAsync(r => r.Users.Select(u => u.UserId).Contains(userId));

    private Task<NewRoom?> GetRoomForUser(string username) =>
        this.rooms.FirstOrDefaultAsync(r => r.Users.Select(u => u.Username).Contains(username));

    private async Task RemovePlayerFromRooms(string username)
    {
        IList<NewRoom> roomList = await this.rooms.Where(r => r.Users.Select(u => u.Username).Contains(username)).ToListAsync();
        foreach (NewRoom room in roomList)
        {
            room.Users.RemoveAll(r => r.Username == username);
            await this.rooms.UpdateAsync(room);
        }
    }

    public Task<IList<NewRoom>> GetRooms
        (Expression<Func<NewRoom, bool>> predicate) =>
        this.rooms.Where(predicate).ToListAsync();

    public async Task AddPlayerToRoom(RoomUser user, NewRoom room)
    {
        NewRoom? currentRoom = await this.GetRoomForUser(user.Username);
        if (currentRoom?.RoomId == room.RoomId) return;

        await this.RemovePlayerFromRooms(user.Username);
        room.Users.Add(user);
        await this.rooms.UpdateAsync(room);
    }

    public async Task<NewRoom> GetOrCreateRoomForUser(UserEntity user)
    {
        NewRoom? room = await this.GetRoomForUser(user.UserId);

        if (room != null) return room;

        room = new NewRoom
        {
            Users = new List<RoomUser>
            {
                new(user.Username, user.UserId),
            },
        };
        await this.rooms.InsertAsync(room);
        return room;
    }

    public Task UpdateRoom(NewRoom room) => this.rooms.UpdateAsync(room);

    public Task RemoveRoom(NewRoom newRoom) => this.rooms.DeleteAsync(newRoom);

    // Insert room

    // Delete room

    // Add player to existing room
}