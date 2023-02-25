using System;
using System.Collections.Generic;
using System.Linq;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using LBPUnion.ProjectLighthouse.Types.Users;
using Redis.OM.Modeling;

namespace LBPUnion.ProjectLighthouse.Types.Redis;

[Document(StorageType = StorageType.Json, Prefixes = new[] {"Room",})]
public class RedisRoom
{
    [RedisIdField]
    [Indexed]
    public Ulid Id { get; set; }

    /// <summary>
    /// The user ID of the host
    /// </summary>
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
    /// playing a community level, or actively diving in.
    /// </summary>
    [Indexed]
    public RoomState RoomState { get; set; }

    /// <summary>
    /// Tracks the current level that the room is playing.
    /// Can be a story level, the pod, a level on the moon, or a community level.
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
    [Searchable]
    public List<int> RoomMembers { get; set; }

    /// <summary>
    /// A map of user ids to their IP address in number form
    /// </summary>
    [Indexed]
    public Dictionary<int, string> MemberLocations { get; set; }

    public bool IsUserInRoom(int userId) => this.RoomMembers.Contains(userId);

}