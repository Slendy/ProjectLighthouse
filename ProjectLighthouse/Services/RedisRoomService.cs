#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Levels;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using Redis.OM;
using Redis.OM.Searching;

namespace LBPUnion.ProjectLighthouse.Services;

public class RedisRoomService : IRoomService
{
    private readonly IRedisCollection<NewRoom> rooms;
    private readonly TimeSpan timeToLive;

    public RedisRoomService(IRedisCollection<NewRoom> rooms, TimeSpan timeToLive)
    {
        this.rooms = rooms;
        this.timeToLive = timeToLive;
    }

    public ValueTask<int> GetPlayerCountAsync() => this.rooms.SumAsync(r => r.Users.Count);

    public Task<int> GetRoomCountAsync() => this.rooms.CountAsync();

    public ValueTask<int> GetPlayerCountForSlotAsync(SlotType slotType, int slotId) =>
        this.rooms.Where(r => r.RoomSlot.SlotType == slotType && r.RoomSlot.SlotId == slotId).SumAsync(r => r.Users.Count);

    // public Task<NewRoom?> GetRoomForUser(int userId) =>
    //     this.rooms.FirstOrDefaultAsync(r => r.Users.Any(u => u.UserId == userId));

    public Task<NewRoom?> GetRoomForUser(string username) =>
        this.rooms.FirstOrDefaultAsync(r => r.Users.Contains(username));

    private async Task RemovePlayerFromRooms(string username)
    {
        IList<NewRoom> roomList =
            await this.rooms.Where(r => r.Users.Contains(username)).ToListAsync();
        foreach (NewRoom room in roomList)
        {
            room.Users.RemoveAll(user => user == username);
            await this.rooms.UpdateAsync(room);
        }
    }

    public Task<NewRoom?> GetRoomForUser(int userId) => throw new NotImplementedException();

    public Task<IList<NewRoom>> GetRooms(Expression<Func<NewRoom, bool>>? predicate = null) =>
        predicate == null 
            ? this.rooms.ToListAsync() 
            : this.rooms.Where(predicate).ToListAsync();

    public async Task AddPlayerToRoom(RoomUser user, NewRoom room)
    {
        NewRoom? currentRoom = await this.GetRoomForUser(user.Username);
        if (currentRoom?.RoomId == room.RoomId) return;

        await this.RemovePlayerFromRooms(user.Username);
        room.Users.Add(user.Username);
        await this.rooms.UpdateAsync(room);
    }

    public async Task<NewRoom> GetOrCreateRoomForUser(UserEntity user)
    {
        NewRoom? room = await this.GetRoomForUser(user.Username);

        if (room != null) return room;

        room = new NewRoom
        {
            Users = new List<string>
            {
                user.Username,
            },
        };
        await this.InsertRoom(room);
        return room;
    }

    public Task InsertRoom(NewRoom room) => this.rooms.InsertAsync(room, this.timeToLive);

    public Task UpdateRoom(NewRoom room) => this.rooms.UpdateAsync(room);

    public Task RemoveRoom(NewRoom newRoom) => this.rooms.DeleteAsync(newRoom);
}