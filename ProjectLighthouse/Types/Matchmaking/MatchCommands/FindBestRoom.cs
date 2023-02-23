#nullable enable
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Redis;
using LBPUnion.ProjectLighthouse.Types.Levels;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using LBPUnion.ProjectLighthouse.Types.Redis;

namespace LBPUnion.ProjectLighthouse.Types.Matchmaking.MatchCommands;

// Schema is the EXACT SAME as CreateRoom (but cant be a subclass here), so see comments there for details
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
public class FindBestRoom : IMatchCommand
{
    public int BuildVersion;
    public int HostMood;
    public int Language;
    public List<int>? Location;

    public List<int>? NAT;
    public int PassedNoJoinPoint;
    public RoomState? RoomState;
    public string? Search;
    public List<string>? Players { get; set; }

    public List<string>? Reservations { get; set; }
    public List<List<int>>? Slots { get; set; }


    public async Task<string?> ProcessCommand(RedisUser user, RedisRoom room, RoomRepository roomRepository, UserRepository userRepository)
    {
        if (user.UserId != room.RoomHostId) return null;
        if (this.Players == null) return null;
        if (this.RoomState == null) return null;
        if (this.Slots == null) return null;

        if (room.RoomBuildVersion == -1) room.RoomBuildVersion = this.BuildVersion;

        RoomSlot slot = new()
        {
            SlotType = (SlotType)this.Slots[0][0],
            SlotId = this.Slots[0][1],
        };

        room.RoomState = (RoomState)this.RoomState;
        room.RoomSlot = slot;

        await roomRepository.SaveAsync();

        //TODO find best room

        return null;
    }
}