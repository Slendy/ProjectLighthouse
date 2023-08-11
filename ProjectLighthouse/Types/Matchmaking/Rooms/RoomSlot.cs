using LBPUnion.ProjectLighthouse.Types.Levels;
using Redis.OM.Modeling;

namespace LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;

public struct RoomSlot
{
    [Indexed]
    public int SlotId { get; set; }

    [Indexed]
    public SlotType SlotType { get; set; }

    public static readonly RoomSlot PodSlot = new()
    {
        SlotType = SlotType.Pod,
        SlotId = 0,
    };
}