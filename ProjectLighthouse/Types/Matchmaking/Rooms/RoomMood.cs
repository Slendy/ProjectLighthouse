namespace LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;

public enum RoomMood : byte
{
    RejectingAll = 0,

    RejectingAllButFriends = 1,

    RejectingOnlyFriends = 2,

    AllowingAll = 3,
}