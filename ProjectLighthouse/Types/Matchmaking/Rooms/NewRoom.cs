using System;
using System.Collections.Generic;
using System.Linq;
using LBPUnion.ProjectLighthouse.Types.Users;
using Redis.OM.Modeling;

namespace LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;

[Document(StorageType = StorageType.Json, Prefixes = new[] { "NewRoom", })]
public class NewRoom
{
    [RedisIdField]
    public Ulid RoomId { get; set; }

    [Indexed]
    public GameVersion RoomVersion { get; set; }

    [Indexed]
    public Platform RoomPlatform { get; set; }

    [Indexed]
    public RoomState RoomState { get; set; }

    [Indexed]
    public RoomMood RoomMood { get; set; }

    [Indexed]
    public RoomSlot RoomSlot { get; set; }

    [Indexed]
    public List<RoomUser> Users { get; set; }
    
    public RoomUser Host => this.Users.First();
}