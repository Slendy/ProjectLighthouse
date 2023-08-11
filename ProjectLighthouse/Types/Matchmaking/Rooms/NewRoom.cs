using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
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

    [Indexed(Sortable = true)]
    public RoomMood RoomMood { get; set; }
    
    [Indexed]
    public RoomSlot RoomSlot { get; set; }

    [Indexed]
    public List<string> Users { get; set; } = new();

    [Indexed(CascadeDepth = 2)]
    public List<UserExpiry> FailedJoin { get; set; } = new();

    [Indexed(CascadeDepth = 2)]
    public List<UserExpiry> RecentlyLeft { get; set; } = new();

    [JsonIgnore]
    public string Host => this.Users.First();
}