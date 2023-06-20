using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using LBPUnion.ProjectLighthouse.Types.Users;
using Redis.OM.Modeling;

namespace LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;

[Document(StorageType = StorageType.Json, Prefixes = new[]{"Room",})]
public class Room
{
    [RedisIdField]
    private Ulid RoomId { get; set; }

    [Indexed]
    public List<string> Players { get; set; }

    [Indexed]
    public GameVersion RoomVersion { get; set; }

    [Indexed]
    public Platform RoomPlatform { get; set; }

    [Indexed]
    public RoomSlot Slot { get; set; }

    [Indexed]
    public RoomState State { get; set; }

    [JsonIgnore]
    [Indexed]
    public bool IsLookingForPlayers => this.State is RoomState.PlayingLevel or RoomState.DivingInWaiting;
}