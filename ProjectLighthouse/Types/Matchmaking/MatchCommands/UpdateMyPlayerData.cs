#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Redis;
using LBPUnion.ProjectLighthouse.Types.Levels;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using LBPUnion.ProjectLighthouse.Types.Redis;

namespace LBPUnion.ProjectLighthouse.Types.Matchmaking.MatchCommands;

public class UpdateMyPlayerData : IMatchCommand
{
    public string Player { get; set; } = "";

    public RoomState? RoomState { get; set; }
    public int? Mood { get; set; }
    public List<int>? Slot { get; set; }
    public bool? PassedNoJoinPoint { get; set; }
    public int? BuildVersion { get; set; }

    public async Task<string?> ProcessCommand(RedisUser user, RedisRoom room, RoomRepository roomRepository, UserRepository userRepository)
    {
        if (this.Player != user.Username) return null;
        if (room.RoomHostId != user.UserId) return null;

        if (this.RoomState != null) room.RoomState = this.RoomState.Value;
        if (this.Slot != null) room.RoomSlot = new RoomSlot {SlotType = (SlotType)this.Slot[0], SlotId = this.Slot[1],};
        if (this.BuildVersion != null) room.RoomBuildVersion = this.BuildVersion.Value;

        await roomRepository.SaveAsync();

        return IMatchCommand.ValidCommand;
    }
}