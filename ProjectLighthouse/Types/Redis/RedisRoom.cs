using System;
using System.ComponentModel.DataAnnotations;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using LBPUnion.ProjectLighthouse.Types.Users;
using Redis.OM.Modeling;

namespace LBPUnion.ProjectLighthouse.Types.Redis;

[Document(StorageType = StorageType.Json, Prefixes = new[] {"User",})]
public class RedisRoom
{
    [Key]
    public Ulid Id { get; set; }

    [Indexed]
    public int RoomHostId { get; set; }

    /// <summary>
    /// Used to compare with other players for matchmaking
    /// </summary>
    [Indexed]
    public GameVersion RoomVersion { get; set; }

    /// <summary>
    /// Used to track whether the room is PSN/RPCN 
    /// </summary>
    [Indexed]
    public Platform RoomPlatform { get; set; }

    /// <summary>
    /// Tracks the current activity of the room, i.e. are they in the pod,
    /// playing a community level, actively diving in.
    /// </summary>
    [Indexed]
    public RoomState RoomState { get; set; }

    /// <summary>
    /// Tracks the current level that the room is playing
    /// Can be a story level, the pod, level on the moon, community level
    /// Potentially used by the client to preload resources when diving in.
    /// </summary>
    [Indexed(CascadeDepth = 1)]
    public RoomSlot RoomSlot { get; set; }

    /// <summary>
    /// Also used during matchmaking to ensure that two players are running the
    /// same build of the game
    /// </summary>
    [Indexed]
    public int RoomBuildVersion { get; set; }

    /// <summary>
    /// Tracks other members of the room that are not the host.
    /// </summary>
    [Indexed]
    public int[] RoomMembers { get; set; }

}