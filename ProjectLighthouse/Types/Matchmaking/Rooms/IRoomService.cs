#nullable enable
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Levels;

namespace LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;

public interface IRoomService
{
    public ValueTask<int> GetPlayerCountAsync();

    public Task<int> GetRoomCountAsync();

    public ValueTask<int> GetPlayerCountForSlotAsync(SlotType slotType, int slotId);

    public Task AddPlayerToRoom(RoomUser user, NewRoom room);

    public Task<NewRoom?> GetRoomForUser(int userId);

    public Task<NewRoom?> GetRoomForUser(string username);

    public Task<IList<NewRoom>> GetRooms(Expression<Func<NewRoom, bool>>? predicate = null);

    public Task<NewRoom> GetOrCreateRoomForUser(UserEntity user);

    public Task InsertRoom(NewRoom room);

    public Task UpdateRoom(NewRoom room);

    public Task RemoveRoom(NewRoom newRoom);
}