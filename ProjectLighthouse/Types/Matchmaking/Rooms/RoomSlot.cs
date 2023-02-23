using System.Collections.Generic;
using LBPUnion.ProjectLighthouse.Types.Levels;

namespace LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;

public class RoomSlot
{
    public int SlotId { get; set; }
    public SlotType SlotType { get; set; }

    public static readonly RoomSlot PodSlot = new()
    {
        SlotType = SlotType.Pod,
        SlotId = 0,
    };

    public static RoomSlot Deserialize(List<List<int>> serializedSlot)
    {
        RoomSlot slot = new()
        {
            SlotType = (SlotType)serializedSlot[0][0],
            SlotId = serializedSlot[0][1],
        };
        return slot;
    }
}